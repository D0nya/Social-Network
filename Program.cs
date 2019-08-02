using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace KataSocialNetwork
{

  public class Program
  {
    static void Main()
    {
      Member a = new Member(new MemberProfile("a", "a", "a", "a"));
      Member b = new Member(new MemberProfile("b", "b", "b", "b"));
      Member c = new Member(new MemberProfile("c", "c", "c", "c"));
      Member d = new Member(new MemberProfile("d", "d", "d", "d"));
      Member e = new Member(new MemberProfile("e", "e", "e", "e"));
      Member f = new Member(new MemberProfile("f", "f", "f", "f"));

      a.AddFriendRequest(b);
      a.ConfirmFriend(b);
      a.AddFriendRequest(c);
      a.ConfirmFriend(c);
      b.AddFriendRequest(e);
      b.AddFriendRequest(f);
      b.ConfirmFriend(e);
      b.ConfirmFriend(f);
      d.AddFriendRequest(f);
      d.ConfirmFriend(f);
      d.AddFriendRequest(a);
      d.ConfirmFriend(a);


      foreach (var item in d.GetFriends(3))
      {
        Console.WriteLine(item.Profile.ToString());
      }
      a.AddPost("asd");
      a.AddPost("dddd");
      a.AddPost("123d");
      b.AddPost("asd11qwea");
      b.AddPost("aWWW");
      c.AddPost("ASdasD");

      foreach (var item in a.GetFeed())
      {
        Console.WriteLine(item.ToString());
      }
      foreach (var item in c.GetFeed())
      {
        Console.WriteLine(item.ToString());
      }
    }
  }

  public class SocialNetwork : ISocialNetwork
  {
    public SocialNetwork()
    {
      Members = new List<IMember>();
    }
    public List<IMember> Members { get; }

    // Adds a new member and returns the added member
    public IMember AddMember(string firstName, string lastName, string city, string country)
    {
      IMember newMember = new Member(new MemberProfile(firstName, lastName, city, country));
      Members.Add(newMember);
      return newMember;
    }

    // Returns the member with the id
    public IMember FindMemberById(int id)
    {
      return Members.FirstOrDefault(memb => memb.Id == id);
    }

    // Returns a list of members by searching all fields in the profile
    public IEnumerable<IMember> FindMember(string search)
    {
      return Members.Where(memb => memb.Profile.ToString().Contains(search)).Select(m => m).ToList();
    }

    // Total number of members currently in the social network
    public int MemberCount { get { return Members.Count; } }
  }
  public class Member : IMember
  {
    private static int memberCount = 1;

    public Member(){}
    public Member(IMemberProfile profile)
    {
      Id = profile.MemberId;
      Profile = profile;
      Friends = new List<IMember>();
      Pending = new List<IMember>();
      Posts = new List<IPost>();
    }

    // Id of member. Must be unique and sequential. 
    public int Id { get; }
    // Member profile
    public IMemberProfile Profile { get; }
    // List of friends
    public IEnumerable<IMember> Friends { get; }

    // List of pending friend requests
    public IEnumerable<IMember> Pending { get; private set; }
    // Members posts
    public IEnumerable<IPost> Posts { get; }

    // Adds a friend request for this member. from - the member making the friend request 
    public void AddFriendRequest(IMember from)
    {
      if ((Pending as List<IMember>).FirstOrDefault(m => m == from) == null && 
          (Friends as List<IMember>).FirstOrDefault(m => m == from) == null &&
           from != this)
          (Pending as List<IMember>).Add(from);
    }

    // Confirms a pending friend request
    public void ConfirmFriend(IMember member)
    {
      if (Pending.FirstOrDefault(s => s == member) != null)
      {
        (Friends as List<IMember>).Add(member);
        (member.Friends as List<IMember>).Add(this);
        (Pending as List<IMember>).Remove(member);
      }
    }

    // Removes a pending friend request
    public void RemoveFriendRequest(IMember member)
    {
      if (Pending.FirstOrDefault(s => s == member) != null)
        (Pending as List<IMember>).Remove(member);
    }

    // Removes a friend
    public void RemoveFriend(IMember member)
    {
      if(Friends.FirstOrDefault(s => s == member) != null)
      {
        (Friends as List<IMember>).Remove(member);
        (member.Friends as List<IMember>).Remove(this);
      }
    }

    // Returns a list of all friends. level - depth (1 - friends, 2 - friends of friends ...)
    public IEnumerable<IMember> GetFriends(int level = 1, IList<int> filter = null)
    {
      List<IMember> list = new List<IMember>();
      if (level <= 1)
        return Friends;
      else
      {
        list = Friends as List<IMember>;
        for (int i = 0; i < level; i++)
        {
          list = list.SelectMany(f => f.Friends).Where(f => f != list.FirstOrDefault()).ToList();
        }
        return list.Distinct();
      }
        
    }

    // Adds a new post to members feed
    public IPost AddPost(string message)
    {
      if (!string.IsNullOrEmpty(message))
      {
        Post post = new Post(this, message);
        (Posts as List<IPost>).Add(post);
        return post;
      }
      return null;
    }

    // Removes the post with the id
    public void RemovePost(int id)
    {
      IPost postToRemove = (Posts as List<IPost>).FirstOrDefault(p => p.Id == id);
      if (postToRemove != null)
        (Posts as List<IPost>).Remove(postToRemove);
    }

    // Returns members feed as a list of posts. Should also return posts of friends.
    public IEnumerable<IPost> GetFeed()
    {
      return (Posts.Union(Friends.SelectMany((f) => f.Posts))).OrderBy(e => e.Id).ToList();
    }

    public static int GetMebmerCount()
    {
      return memberCount++;
    }
  }
  public class MemberProfile : IMemberProfile
  {
    public MemberProfile() { }
    public MemberProfile(string firstName, string lastName, string city, string country)
    {
      Firstname = firstName;
      Lastname = lastName;
      City = city;
      Country = country;

      MemberId = Member.GetMebmerCount();
    }

    // Id of the Member this profile belongs to
    public int MemberId { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

    public override string ToString()
    {
      return $"[{MemberId}, {Firstname}, {Lastname}, {City}, {Country}]";
    }
  }
  public class Post : IPost
  {
    public Post(){}
    public Post(IMember member, string message)
    {
      Member = member;
      Message = message;
      Date = DateTime.Now;
      Likes = 0;
      Id = member.GetFeed().Count() + 1;
    }

    // Id of post. Must be unique and sequential.
    public int Id { get; set; }
    // Member that made this post
    public IMember Member { get; set; }
    // The post message
    public string Message { get; set; }
    // Date and time post was made
    public DateTime Date { get; set; }
    // Likes for post
    public int Likes { get; set; }
    public override string ToString()
    {
      return $"[Id: {Id}, Member: {Member.Profile.Firstname} {Member.Profile.Lastname}, {Date}, Likes: {Likes}\nMessage:\n{Message}";
    }
  }
}