﻿using System;
using System.Runtime.ExceptionServices;
using VimeoDotNet.Extensions;
using VimeoDotNet.Models;
using VimeoDotNet.Net;
using VimeoDotNet.Parameters;

namespace VimeoDotNet
{
    /// <inheritdoc />
    /// <summary>
    /// Implementation of Vimeo API
    /// </summary>
    public partial class VimeoClient
    {
        #region User authentication

        /// <inheritdoc />
        /// <summary>
        /// Exchange the code for an access token
        /// </summary>
        /// <param name="authorizationCode">A string token you must exchange for your access token</param>
        /// <param name="redirectUrl">This field is required, and must match one of your application’s
        /// redirect URI’s</param>
        /// <returns>AccessTokenResponse</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public AccessTokenResponse GetAccessToken(string authorizationCode, string redirectUrl)
        {
            return OAuth2Client.GetAccessTokenAsync(authorizationCode, redirectUrl).RunSynchronouslyWithCurrentCulture();
        }
        #endregion

        #region Account information

        /// <inheritdoc />
        /// <summary>
        /// Get user information
        /// </summary>
        /// <returns>User information</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public User GetAccountInformation()
        {
            try
            {
                return GetAccountInformationAsync().RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Update user information
        /// </summary>
        /// <param name="parameters">User parameters</param>
        /// <returns>User information</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public User UpdateAccountInformation(EditUserParameters parameters)
        {
            try
            {
                return UpdateAccountInformationAsync(parameters).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get user information
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns>User information object</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public User GetUserInformation(long userId)
        {
            try
            {
                return GetUserInformationAsync(userId).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        #endregion

        #region Albums

        /// <inheritdoc />
        /// <summary>
        /// Get album by parameters
        /// </summary>
        /// <param name="parameters">GetAlbumsParameters</param>
        /// <returns>Paginated albums</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public Paginated<Album> GetAlbums(GetAlbumsParameters parameters = null)
        {
            try
            {
                return GetAlbumsAsync(parameters).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get album by UserId and parameters
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="parameters">GetAlbumsParameters</param>
        /// <returns>Paginated albums</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public Paginated<Album> GetAlbums(long userId, GetAlbumsParameters parameters = null)
        {
            try
            {
                return GetAlbumsAsync(userId, parameters).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get album by AlbumId
        /// </summary>
        /// <param name="albumId">AlbumId</param>
        /// <returns>Album</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public Album GetAlbum(long albumId)
        {
            try
            {
                return GetAlbumAsync(albumId).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        ///  <summary>
        /// Get album by AlbumId and UserId
        ///  </summary>
        ///  <param name="userId">AlbumId</param>
        ///  <param name="albumId">UserId</param>
        ///  <returns>Album</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public Album GetAlbum(long userId, long albumId)
        {
            try
            {
                return GetAlbumAsync(userId, albumId).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create new album
        /// </summary>
        /// <param name="parameters">Creation parameters</param>
        /// <returns>Album</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public Album CreateAlbum(EditAlbumParameters parameters = null)
        {
            try
            {
                return CreateAlbumAsync(parameters).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Update album
        /// </summary>
        /// <param name="albumId">Albumid</param>
        /// <param name="parameters">Album parameters</param>
        /// <returns>Album</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public Album UpdateAlbum(long albumId, EditAlbumParameters parameters = null)
        {
            try
            {
                return UpdateAlbumAsync(albumId, parameters).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Delete album
        /// </summary>
        /// <param name="albumId">AlbumId</param>
        /// <returns>Deletion result</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public bool DeleteAlbum(long albumId)
        {
            try
            {
                return DeleteAlbumAsync(albumId).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return false;
        }


        /// <inheritdoc />
        /// <summary>
        /// Add video to album by AlbumId and ClipId
        /// </summary>
        /// <param name="albumId">AlbumId</param>
        /// <param name="clipId">ClipId</param>
        /// <returns>Adding result</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public bool AddToAlbum(long albumId, long clipId)
        {
            try
            {
                return AddToAlbumAsync(albumId, clipId).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return false;
        }

        /// <inheritdoc />
        /// <summary>
        /// Add video to album by UserId and AlbumId and ClipId
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="albumId">AlbumId</param>
        /// <param name="clipId">ClipId</param>
        /// <returns>Adding result</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public bool AddToAlbum(long userId, long albumId, long clipId)
        {
            try
            {
                return AddToAlbumAsync(userId, albumId, clipId).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return false;
        }


        /// <inheritdoc />
        /// <summary>
        /// Remove video from album by AlbumId and ClipId
        /// </summary>
        /// <param name="albumId">AlbumId</param>
        /// <param name="clipId">ClipId</param>
        /// <returns>Removing result</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public bool RemoveFromAlbum(long albumId, long clipId)
        {
            try
            {
                return RemoveFromAlbumAsync(albumId, clipId).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return false;
        }

        /// <inheritdoc />
        /// <summary>
        /// Remove video from album by AlbumId and ClipId and UserId
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="albumId">AlbumId</param>
        /// <param name="clipId">ClipId</param>
        /// <returns>Removing result</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public bool RemoveFromAlbum(long userId, long albumId, long clipId)
        {
            try
            {
                return RemoveFromAlbumAsync(userId, albumId, clipId).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return false;
        }

        #endregion

        #region Videos

        /// <inheritdoc />
        /// <summary>
        /// Get paginated video for current account
        /// </summary>
        /// <returns>Paginated videos</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public Paginated<Video> GetVideos(string[] fields = null)
        {
            try
            {
                return GetVideosAsync(fields:fields).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get video by ClipId
        /// </summary>
        /// <param name="clipId">ClipId</param>
        /// <param name="fields"></param>
        /// <returns>Video</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public Video GetVideo(long clipId, string[] fields = null)
        {
            try
            {
                return GetVideoAsync(clipId, fields).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get videos  by UserId and query
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="query">Search query</param>
        /// <param name="fields"></param>
        /// <returns>Paginated videos</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public Paginated<Video> GetUserVideos(long userId, string query = null, string[] fields = null)
        {
            return GetUserVideos(userId, null, null, query, fields);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get videos by UserId and query and page parameters
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="perPage">Number of items to show on each page. Max 50</param>
        /// <param name="query">Search query</param>
        /// <param name="page">The page number to show</param>
        /// <param name="fields"></param>
        /// <returns>Paginated videos</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public Paginated<Video> GetUserVideos(long userId, int? page, int? perPage, string query = null, string[] fields = null)
        {
            try
            {
                return GetUserVideosAsync(userId, page, perPage, query, fields).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get video by ClipId for UserId
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="clipId">ClipId</param>
        /// <param name="fields"></param>
        /// <returns>Video</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public Video GetUserVideo(long userId, long clipId, string[] fields = null)
        {
            try
            {
                return GetUserVideoAsync(userId, clipId, fields).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get videos by AlbumId
        /// </summary>
        /// <param name="albumId">AlbumId</param>
        /// <param name="page">The page number to show</param>
        /// <param name="perPage">Number of items to show on each page. Max 50</param>
        /// <param name="sort">The default sort order of an Album's videos</param>
        /// <param name="direction">The direction that the results are sorted</param>
        /// <param name="fields"></param>
        /// <returns>Paginated videos</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public Paginated<Video> GetAlbumVideos(long albumId, int? page, int? perPage, string sort = null, string direction = null, string[] fields = null)
        {
            try
            {
                return GetAlbumVideosAsync(albumId, page, perPage, sort, direction, fields).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get video from album by AlbumId and ClipId
        /// </summary>
        /// <param name="albumId">AlbumId</param>
        /// <param name="clipId">ClipId</param>
        /// <param name="fields"></param>
        /// <returns>Video</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public Video GetAlbumVideo(long albumId, long clipId, string[] fields = null)
        {
            try
            {
                return GetAlbumVideoAsync(albumId, clipId, fields).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get videos from album by AlbumId and UserId
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <param name="albumId">AlbumId</param>
        /// <param name="fields"></param>
        /// <returns>Paginated videos</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public Paginated<Video> GetUserAlbumVideos(long userId, long albumId, string[] fields = null)
        {
            try
            {
                return GetUserAlbumVideosAsync(userId, albumId, fields).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get video from album by AlbumId and UserId and ClipId
        /// </summary>
        /// <param name="userId">AlbumId</param>
        /// <param name="albumId">UserId</param>
        /// <param name="clipId">ClipId</param>
        /// <param name="fields"></param>
        /// <returns>Video</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public Video GetUserAlbumVideo(long userId, long albumId, long clipId, string[] fields = null)
        {
            try
            {
                return GetUserAlbumVideoAsync(userId, albumId, clipId, fields).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Update video metadata by ClipId
        /// </summary>
        /// <param name="clipId">ClipId</param>
        /// <param name="metaData">New video metadata</param>
        [Obsolete("Use async API instead sync wrapper")]
        public void UpdateVideoMetadata(long clipId, VideoUpdateMetadata metaData)
        {
            try
            {
                UpdateVideoMetadataAsync(clipId, metaData).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Update allowed domain for clip
        /// </summary>
        /// <param name="clipId">ClipId</param>
        /// <param name="domain">Domain</param>
        [Obsolete("Use async API instead sync wrapper")]
        public void UpdateVideoAllowedDomain(long clipId, string domain)
        {
            try
            {
                UpdateVideoAllowedDomainAsync(clipId, domain).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Delete video
        /// </summary>
        /// <param name="clipId">CliepId</param>
        [Obsolete("Use async API instead sync wrapper")]
        public void DeleteVideo(long clipId)
        {
            try
            {
                DeleteVideoAsync(clipId).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
        }

        #endregion

        #region Upload files

        /// <inheritdoc />
        /// <summary>
        /// Create new upload ticket for replace video
        /// </summary>
        /// <param name="videoId">VideoId</param>
        /// <returns>Upload ticket</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public UploadTicket GetReplaceVideoUploadTicket(long videoId)
        {
            try
            {
                return GetReplaceVideoUploadTicketAsync(videoId).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create new upload ticket
        /// </summary>
        /// <returns>Upload ticket</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public UploadTicket GetUploadTicket()
        {
            try
            {
                return GetUploadTicketAsync().RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Start upload file
        /// </summary>
        /// <param name="fileContent">FileContent</param>
        /// <param name="chunkSize">ChunkSize</param>
        /// <param name="replaceVideoId">ReplaceVideoId</param>
        /// <returns>Upload request</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public IUploadRequest StartUploadFile(IBinaryContent fileContent, int chunkSize = DEFAULT_UPLOAD_CHUNK_SIZE, long? replaceVideoId = null)
        {
            try
            {
                return StartUploadFileAsync(fileContent, chunkSize, replaceVideoId).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Upload file part
        /// </summary>
        /// <param name="fileContent">FileContent</param>
        /// <param name="chunkSize">ChunkSize</param>
        /// <param name="replaceVideoId">ReplaceVideoId</param>
        /// <returns>Upload request</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public IUploadRequest UploadEntireFile(IBinaryContent fileContent, int chunkSize = DEFAULT_UPLOAD_CHUNK_SIZE, long? replaceVideoId = null)
        {
            try
            {
                return UploadEntireFileAsync(fileContent, chunkSize, replaceVideoId).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Continue upload file
        /// </summary>
        /// <param name="uploadRequest">UploadRequest</param>
        /// <returns>Verification upload response</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public VerifyUploadResponse ContinueUploadFile(IUploadRequest uploadRequest)
        {
            try
            {
                return ContinueUploadFileAsync(uploadRequest).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Verify upload file part
        /// </summary>
        /// <param name="uploadRequest">UploadRequest</param>
        /// <returns>Verification reponse</returns>
        [Obsolete("Use async API instead sync wrapper")]
        public VerifyUploadResponse VerifyUploadFile(IUploadRequest uploadRequest)
        {
            try
            {
                return VerifyUploadFileAsync(uploadRequest).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Complete upload file
        /// </summary>
        /// <param name="uploadRequest">UploadRequest</param>
        /// <returns></returns>
        [Obsolete("Use async API instead sync wrapper")]
        public void CompleteFileUpload(IUploadRequest uploadRequest)
        {
            try
            {
                CompleteFileUploadAsync(uploadRequest).RunSynchronouslyWithCurrentCulture();
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Upload thumbnail
        /// </summary>
        /// <param name="clipId"></param>
        /// <param name="fileContent"></param>
        [Obsolete("Use async API instead sync wrapper")]
        public Picture UploadThumbnail(long clipId, IBinaryContent fileContent)
        {
            try
            {
                var pic = UploadPictureAsync(fileContent, clipId).RunSynchronouslyWithCurrentCulture();
                SetThumbnailActiveAsync(pic.uri).RunSynchronouslyWithCurrentCulture();
                return pic;
            }
            catch (AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return null;
            }
        }

        #endregion
    }
}