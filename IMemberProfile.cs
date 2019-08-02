namespace KataSocialNetwork
{
  public interface IMemberProfile
  {
    int MemberId { get; set; }
    string Firstname { get; set; }
    string Lastname { get; set; }
    string City { get; set; }
    string Country { get; set; }
  }
}