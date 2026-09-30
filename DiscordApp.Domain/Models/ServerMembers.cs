using System;
using System.Text.Json.Serialization.Metadata;

namespace DiscordApp.Domain.Models;

public class ServerMembers
{
    public int ServerId{get; set;}
    public int UserId{get; set;}
}
