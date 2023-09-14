namespace TwitPoster.BLL.External.Location;

public record CountriesResponse (bool Error, string Msg, List<CountryResponseItem> Data);
public record StatesResponse (bool Error, string Msg, StatesResponseItem Data);
public record CitiesResponse (bool Error, string Msg, List<string> Data);
public record StatesResponseItem(string Name, string Iso2, string Iso3, List<StateResponseItem> States);
public record StateResponseItem(string Name, string State_Code);
public record CountryResponseItem (string Name, string Iso2, string Iso3, string UnicodeFlag);
