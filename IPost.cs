using System;

namespace KataSocialNetwork
{
  public interface IPost
  {
    // Id of post. Must be unique and sequential.
    int Id { get; set; }
    // Member that made this post
    IMember Member { get; set; }
    // The post message
    string Message { get; set; }
    // Date and time post was made
    DateTime Date { get; set; }
    // Likes for post
    int Likes { get; set; }
  }
}