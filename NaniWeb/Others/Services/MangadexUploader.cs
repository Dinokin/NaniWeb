using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NaniWeb.Data;

namespace NaniWeb.Others.Services
{
    public class MangadexUploader
    {
        public const string MangadexAddress = "https://mangadex.org";
        private const string MangadexLoginAddress = MangadexAddress + "/ajax/actions.ajax.php?function=login";
        private const string MangadexLogoutAddress = MangadexAddress + "/ajax/actions.ajax.php?function=logout ";
        private const string MangadexChapterUploadAddress = MangadexAddress + "/ajax/actions.ajax.php?function=chapter_upload";
        private const string MangadexChapterEditAddress = MangadexAddress + "/ajax/actions.ajax.php?function=chapter_edit";
        private const string MangadexSeriesPage = MangadexAddress + "/title";
        private readonly HttpClient _client;

        private readonly SettingsKeeper _settingsKeeper;

        public MangadexUploader(SettingsKeeper settingsKeeper, IHttpClientFactory httpClientFactory)
        {
            _settingsKeeper = settingsKeeper;
            if (bool.Parse(_settingsKeeper.GetSetting("EnableDiscordBot").Value))
                _client = httpClientFactory.CreateClient("MangadexClient");
        }

        public async Task<MangadexChapter> UploadChapter(Series series, Chapter chapter, MangadexSeries mangadexSeries, Stream stream)
        {
            if (!bool.Parse(_settingsKeeper.GetSetting("EnableDiscordBot").Value))
                return new MangadexChapter
                {
                    ChapterId = chapter.Id,
                    MangadexId = -1,
                    Chapter = chapter
                };

            var request = new MultipartFormDataContent();
            var mangaId = new StringContent(mangadexSeries.MangadexId.ToString());
            var chapterName = new StringContent(chapter.Name);
            var volumeNumber = new StringContent(chapter.Volume > 0 ? chapter.Volume.ToString() : string.Empty);
            var chapterNumber = new StringContent(chapter.ChapterNumber.ToString(CultureInfo.InvariantCulture));
            var groupId = new StringContent(_settingsKeeper.GetSetting("MangadexGroupId").Value);
            var groupId2 = new StringContent(string.Empty);
            var groupId3 = new StringContent(string.Empty);
            var langId = new StringContent("1");
            var fileStream = new StreamContent(stream);

            mangaId.Headers.ContentType = null;
            chapterName.Headers.ContentType = null;
            volumeNumber.Headers.ContentType = null;
            chapterNumber.Headers.ContentType = null;
            groupId.Headers.ContentType = null;
            groupId2.Headers.ContentType = null;
            groupId3.Headers.ContentType = null;
            langId.Headers.ContentType = null;
            fileStream.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse("form-data; name=\"file\"; filename=\"file.zip\"");
            fileStream.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

            request.Add(mangaId, "\"manga_id\"");
            request.Add(chapterName, "\"chapter_name\"");
            request.Add(volumeNumber, "\"volume_number\"");
            request.Add(chapterNumber, "\"chapter_number\"");
            request.Add(groupId, "\"group_id\"");
            request.Add(groupId2, "\"group_id_2\"");
            request.Add(groupId3, "\"group_id_3\"");
            request.Add(langId, "\"lang_id\"");
            request.Add(fileStream);

            await Login();
            await _client.PostAsync(MangadexChapterUploadAddress, request);
            await Logout();

            return await GetChapterId(chapter, mangadexSeries);
        }

        public async Task UpdateChapter(Chapter chapter, MangadexSeries mangadexSeries, MangadexChapter mangadexChapter, Stream stream)
        {
            if (bool.Parse(_settingsKeeper.GetSetting("EnableDiscordBot").Value))
            {
                var request = new MultipartFormDataContent();
                var mangaId = new StringContent(mangadexSeries.MangadexId.ToString());
                var chapterName = new StringContent(chapter.Name);
                var volumeNumber = new StringContent(chapter.Volume > 0 ? chapter.Volume.ToString() : string.Empty);
                var chapterNumber = new StringContent(chapter.ChapterNumber.ToString(CultureInfo.InvariantCulture));
                var groupId = new StringContent(_settingsKeeper.GetSetting("MangadexGroupId").Value);
                var groupId2 = new StringContent(string.Empty);
                var groupId3 = new StringContent(string.Empty);
                var langId = new StringContent("1");
                var fileStream = new StreamContent(stream);

                mangaId.Headers.ContentType = null;
                chapterName.Headers.ContentType = null;
                volumeNumber.Headers.ContentType = null;
                chapterNumber.Headers.ContentType = null;
                groupId.Headers.ContentType = null;
                groupId2.Headers.ContentType = null;
                groupId3.Headers.ContentType = null;
                langId.Headers.ContentType = null;
                fileStream.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse("form-data; name=\"file\"; filename=\"file.zip\"");
                fileStream.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

                request.Add(mangaId, "\"manga_id\"");
                request.Add(chapterName, "\"chapter_name\"");
                request.Add(volumeNumber, "\"volume_number\"");
                request.Add(chapterNumber, "\"chapter_number\"");
                request.Add(groupId, "\"group_id\"");
                request.Add(groupId2, "\"group_id_2\"");
                request.Add(groupId3, "\"group_id_3\"");
                request.Add(langId, "\"lang_id\"");
                request.Add(fileStream);

                await Login();
                await _client.PostAsync($"{MangadexChapterEditAddress}&id={mangadexChapter.MangadexId}", request);
                await Logout();
            }
        }

        private async Task Login()
        {
            var request = new MultipartFormDataContent();
            var user = new StringContent(_settingsKeeper.GetSetting("MangadexUser").Value);
            var pass = new StringContent(_settingsKeeper.GetSetting("MangadexPassword").Value);
            var twoFA = new StringContent(string.Empty);
            var rememberMe = new StringContent("1");

            user.Headers.ContentType = null;
            pass.Headers.ContentType = null;
            twoFA.Headers.ContentType = null;
            rememberMe.Headers.ContentType = null;

            request.Add(user, "\"login_username\"");
            request.Add(pass, "\"login_password\"");
            request.Add(twoFA, "\"two_factor\"");
            request.Add(rememberMe, "\"remember_me\"");

            await _client.PostAsync(MangadexLoginAddress, request);
        }

        private async Task Logout()
        {
            await _client.PostAsync(MangadexLogoutAddress, null);
        }

        private async Task<MangadexChapter> GetChapterId(Chapter chapter, MangadexSeries mangadexSeries)
        {
            var html = new HtmlWeb {UserAgent = "NaniWeb/1.0"};
            int? mangadexId;

            do
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                var parseResult = int.TryParse((await html.LoadFromWebAsync($"{MangadexSeriesPage}/{mangadexSeries.MangadexId}")).DocumentNode.Descendants("div")
                    .Where(element => element.Attributes.Contains("data-group") && element.GetAttributeValue("data-group", string.Empty) == _settingsKeeper.GetSetting("MangadexGroupId").Value)
                    .Single(element => element.GetAttributeValue("data-chapter", string.Empty) == chapter.ChapterNumber.ToString(CultureInfo.InvariantCulture))
                    .GetAttributeValue("data-id", string.Empty), out var result);

                mangadexId = parseResult ? result : (int?) null;
            } while (mangadexId == null);

            return new MangadexChapter {Chapter = chapter, ChapterId = chapter.Id, MangadexId = mangadexId.Value};
        }
    }
}