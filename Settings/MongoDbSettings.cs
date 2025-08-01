﻿namespace Common.Library.Settings;

public class MongoDbSettings
{
    public string Host { get; init; }
    public int Port { get; init; }
    public string ConnectionString => $"mongodb://{Host}:{Port}";
}
