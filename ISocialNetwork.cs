using System.Collections.Generic;

namespace KataSocialNetwork
{
  public interface ISocialNetwork
  {
    // Adds a new member and returns the added member
    IMember AddMember(string firstName, string lastName, string city, string country);
    // Returns the member with the id
    IMember FindMemberById(int id);
    // Returns a list of members by searching all fields in the profile
    IEnumerable<IMember> FindMember(string search);
    // Total number of members currently in the social network
    int MemberCount { get; }
  }
}