using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Application.CQRS
{
    public record GetLibraryQuery(Guid UserId) : IRequest<List<LibraryDTO>>;

    public record SearchPodcastQuery(string Query, int Page, int PageSize) : IRequest<List<PodcastSearchResultDTO>>;
    public record GetPodcastDetailsQuery(Guid PodcastId) : IRequest<PodcastDetailsDTO>;
    public record GetPlaybackProgressQuery(Guid UserId, Guid EpisodeId) : IRequest<PlaybackProgressDTO>;
}
