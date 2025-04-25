using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Common.Models
{
    public class DiscordUser
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("username")]
        public ulong Username { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("avatar_decoration")]
        public string AvatarDecoration { get; set; }

        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }

        [JsonProperty("public_flags")]
        public int PublicFlags { get; set; }
    }

    public class DiscordGuildMember
    {
        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("communication_disabled_until")]
        public DateTimeOffset? TimedOutUntil { get; set; }

        [JsonProperty("flags")]
        public int Flags { get; set; }

        [JsonProperty("is_pending")]
        public bool IsPending { get; set; }

        [JsonProperty("joined_at")]
        public DateTimeOffset JoinedAt { get; set; }

        [JsonProperty("nick")]
        public string Nick { get; set; }

        [JsonProperty("pending")]
        public bool Pending { get; set; }

        [JsonProperty("premium_since")]
        public DateTimeOffset? PremiumSince { get; set; }

        [JsonProperty("roles")]
        public List<string> Roles { get; set; }

        [JsonProperty("user")]
        public DiscordUser User { get; set; }

        [JsonProperty("mute")]
        public bool Mute { get; set; }

        [JsonProperty("deaf")]
        public bool Deaf { get; set; }
    }
}