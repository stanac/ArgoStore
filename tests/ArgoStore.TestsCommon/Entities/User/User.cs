﻿namespace ArgoStore.TestsCommon.Entities.User;

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public UserContact Contact { get; set; }
}