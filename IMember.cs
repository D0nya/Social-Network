using System.Collections.Generic;

namespace KataSocialNetwork
{
  public interface IMember
  {
    // Id of member. Must be unique and sequential. 
    int Id { get; }
    // Member profile
    IMemberProfile Profile { get; }
    // List of friends
    IEnumerable<IMember> Friends { get; }
    // List of pending friend requests
    IEnumerable<IMember> Pending { get; }
    // Members posts
    IEnumerable<IPost> Posts { get; }

    // Adds a friend request for this member. from - the member making the friend request 
    void AddFriendRequest(IMember from);

    // Confirms a pending friend request
    void ConfirmFriend(IMember member);

    // Removes a pending friend request
    void RemoveFriendRequest(IMember member);

    // Removes a friend
    void RemoveFriend(IMember member);

    // Returns a list of all friends. level - depth (1 - friends, 2 - friends of friends ...)
    IEnumerable<IMember> GetFriends(int level = 1, IList<int> filter = null);

    // Adds a new post to members feed
    IPost AddPost(string message);

    // Removes the post with the id
    void RemovePost(int id);

    // Returns members feed as a list of posts. Should also return posts of friends.
    IEnumerable<IPost> GetFeed();
  }
}