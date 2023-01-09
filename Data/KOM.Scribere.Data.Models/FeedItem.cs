﻿namespace KOM.Scribere.Data.Models;

using KOM.Scribere.Data.Common.Models;

public class FeedItem : BaseDeletableModel<int>
{
    public string Title { get; set; }

    public string Url { get; set; }

    public bool IsRead { get; set; }

    public int FeedId { get; set; }

    public virtual Feed Feed { get; set; }
}
