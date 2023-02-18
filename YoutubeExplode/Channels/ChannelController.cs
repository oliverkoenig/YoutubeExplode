﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode.Bridge;
using YoutubeExplode.Exceptions;

namespace YoutubeExplode.Channels;

internal class ChannelController
{
    private readonly HttpClient _http;

    public ChannelController(HttpClient http) => _http = http;

    private async ValueTask<ChannelPage> GetChannelPageAsync(
        string channelRoute,
        CancellationToken cancellationToken = default)
    {
        var retriesRemaining = 5;
        while (true)
        {
            var channelPage = ChannelPage.TryParse(
                await _http.GetStringAsync(channelRoute, cancellationToken)
            );

            if (channelPage is null)
            {
                if (retriesRemaining-- > 0)
                    continue;

                throw new YoutubeExplodeException(
                    "Channel page is broken. " +
                    "Please try again in a few minutes."
                );
            }

            return channelPage;
        }
    }

    public async ValueTask<ChannelPage> GetChannelPageAsync(
        ChannelId channelId,
        CancellationToken cancellationToken = default) =>
        await GetChannelPageAsync("channel/" + channelId, cancellationToken);

    public async ValueTask<ChannelPage> GetChannelPageAsync(
        UserName userName,
        CancellationToken cancellationToken = default) =>
        await GetChannelPageAsync("user/" + userName, cancellationToken);

    public async ValueTask<ChannelPage> GetChannelPageAsync(
        ChannelSlug channelSlug,
        CancellationToken cancellationToken = default) =>
        await GetChannelPageAsync("c/" + channelSlug, cancellationToken);

    public async ValueTask<ChannelPage> GetChannelPageAsync(
        ChannelHandle channelHandle,
        CancellationToken cancellationToken = default) =>
        await GetChannelPageAsync("@" + channelHandle, cancellationToken);
}