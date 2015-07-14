<Query Kind="Statements" />

Random r = new Random();
string alphabet = "abcdefghijklmnopqrstuvwyxzeeeiouea";
Func<char> randomLetter = () => alphabet[r.Next(alphabet.Length)];
Func<int, string> makeName = 
  (length) => new string(Enumerable.Range(0, length)
     .Select(x => x==0 ? char.ToUpper(randomLetter()) : randomLetter())
     .ToArray());
	 
makeName(8).Dump();