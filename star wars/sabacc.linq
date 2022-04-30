<Query Kind="Program" />

// 2 Pots (Hand pot and Sabacc pot)
// Players could designate a dealer or take turns as the dealer - No need, the Computer/Server will be the Dealer
// The object of sabacc was to have a final hand with a total as close to 23 or -23 as possible without going over. 
// A hand with a total of 24 or higher was said to "bomb out" and lost the round - Visa versa -24??

// A perfect hand of 23 or −23 was called a Pure Sabacc, and it could only be beaten by a rare and unbeatable hand called an Idiot's Array, consisting of The Idiot, a 2 card of any suit, and a 3 card of any suit.

// DEALING:
// The dealer started by shuffling the deck before dealing one card to each player
// Repeat another rotation until each player had two cards facedown.


// The hand ends when a player (after the pot-building phase) declares they would like to Call the hand.

// The penalty for Bombing Out is to pay an amount equal to the contents of the Main Pot into the Sabacc Pot. The winning player takes the contents of the Main Pot. If that player won with a Pure Sabacc or an Idiot’s Array, the Sabacc Pot is also collected.

void Main()
{
	Players players = new();
	players.Register(new Player("Player 1", 100m));
	players.Register(new Player("Player 2", 100m));
	players.Register(new Player("Player 3", 100m));
	players.Register(new Player("Player 4", 100m));

	var session = new Session(players, bombOutFixedPenalty: 10m, autoBetAmount: 10.0m);

	/*	
	
	Two cards dealt to each player
	Hand totals declared
	Play Round (dice rolled after each player completes their play)
	Betting Round (dice rolled at the conclusion of the Betting Round)
	Hand is Called during any Betting Round after the third round.	
	*/

	session.ShuffleDeck();
	session.Ante();
	session.Deal(passes: 2);
	session.DeclareHandTotals();

	while (session.HasWinner is not true)
	{
		session.PlayRound();

		// The player to the dealer’s left then places the first bet into the Main Pot, and a Betting Round proceeds, with players raising as they see fit. 
		// The Betting Round ends when every player has either called the bet or folded. 
		// To signal the end of the Betting Round, the dealer rolls the dice: If they pair, a Sabacc Shift occurs.
		session.BettingRound();

		session.EndRound();
	}


	// The dice are rolled once after :
	// 	- the completion of every Betting Round, 
	//  - after each player makes a play during the Play Round, 
	//  - and once after the game is Called after final bets are placed but before players reveal their cards. 
	// 
	// This allows for a Shift to occur during every critical moment of the game, but the probability of a Shift occurring at any point remains relatively low. 
	// If a Sabacc Shift does occur, the dealer collects all the cards that are not locked, shuffles them into the deck, then deals the cards back to the players. 
	// Players should end a Sabacc shift with the same amount of cards in their hands as they had before the Shift.


	// Each hand starts with every player placing an ante into both the Hand and Sabacc Pots
	// The Dealer then deals out two cards, one at a time, to each player. Starting to the left of the dealer, each player calls out their beginning hand totals.
	// Next begins a Play Round. In this round, players can draw one card from the deck, trade a card from their hand for a card from the deck, or stand. Players cannot discard cards. 
	// Each player rolls the dice after they finish their turn in the play round. If the dice ever pair, a Sabacc Shift occurs.

	// It is only during this round that players can place cards into the Disruptor Field to prevent them from being Shifted should a Shift occur. 
	// Players do this simply by placing their cards face-up in front of them. Players may only place one card per turn in the disruptor field, but may have as many cards total in the Field as they desire. 
	// The downside is that they may only remove one card per turn from the Field as well. 
	// Players may do both, though, remove one card from and place one card into the Field per turn.

	// The player to the dealer’s left then places the first bet into the Main Pot, and a Betting Round proceeds, with players raising as they see fit. 
	// The Betting Round ends when every player has either called the bet or folded. To signal the end of the Betting Round, the dealer rolls the dice. If they pair, a Sabacc Shift occurs.

	// The Hand can only be Called during the fourth Betting Round or any Betting Round thereafter. 
	// This allows for a brief period for the pot to build. 
	// After a player Calls the Hand, the Betting Round proceeds just like normal. 
	// After the final Betting Round is concluded, the dice are rolled one last time, and a Sabacc Shift occurs should they match. Then players reveal their cards.

	session.Dump();
}

class Players : IEnumerable<Player>
{
	List<Player> players = new List<Player>();

	public Players()
	{

	}

	public IEnumerator<Player> GetEnumerator()
	{
		return players.GetEnumerator();
	}

	public void Register(Player player)
	{
		players.Add(player);
	}

	public void Ante(Pot hand, Pot sabacc)
	{
		foreach (var player in this)
		{
			player.Bet(hand, Util.ReadLine<int>("Ante amount for hand pot"));
			player.Bet(sabacc, Util.ReadLine<int>("Ante amount for sabacc pot"));
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return players.GetEnumerator();
	}
}

interface ISabaccTable
{
	public void Protect(Player player, ICard card);
	public void Register(Player player);
}

enum BetActions
{
	Call, // Only after the 3rd betting round
	Fold,
	Bet
}

class Session
{
	public decimal? AutoBetAmount { get; }
	public BombOutPenaltyStrategy BombOutPenaltyStrategy { get; }

	public Dealer Dealer { get; }

	public Players Players { get; }

	public Pot Hand { get; }
	public Pot Sabacc { get; }


	public decimal MinBet { get; private set; }
	public int Rounds { get; private set; }
	public int MinRoundsForCall { get; }

	public void EndRound()
	{
		MinBet *= 2;
		Rounds++;
	}

	public decimal? FixedPenalty { get; }


	public bool HasWinner { get; private set; }

	public void ShuffleDeck()
	{
		Dealer.ShuffleDeck();
	}

	public void Ante()
	{
		foreach (var player in Players)
		{
			player.Bet(Hand, AutoBetAmount ?? Util.ReadLine<decimal>("Hand pot amount"));
			player.Bet(Sabacc, AutoBetAmount ?? Util.ReadLine<decimal>("Sabacc pot amount"));
		}
	}

	public void Deal(int passes)
	{
		for (int i = 1; i <= passes; i++)
		{
			foreach (var player in Players)
			{
				Dealer.Deal(player);
			}
		}
	}

	internal void DeclareHandTotals()
	{
		foreach (var player in Players)
		{
			player.DeclareTotal();
		}
	}

	internal void PlayRound()
	{
		foreach (var player in Players)
		{
			Dealer.Play(player);

			Dealer.Dice.Roll();

			if (Dealer.Dice.IsSabaccShift())
			{
				Dealer.SabaccShift();
			}
		}
	}

	// The player to the dealer’s left then places the first bet into the Main Pot, and a Betting Round proceeds, with players raising as they see fit. 
	// The Betting Round ends when every player has either called the bet or folded. 
	// To signal the end of the Betting Round, the dealer rolls the dice: If they pair, a Sabacc Shift occurs.
	internal void BettingRound()
	{
		foreach (var player in Players)
		{
			if (Rounds >= 4)
			{
				var amount = Util.ReadLine<decimal?>("Bet amount or empty to fold");
			}
			else
			{
				var amount = Util.ReadLine<decimal?>("Bet amount or empty to fold");

				if (amount.HasValue)
				{
					player.Bet(Hand, AutoBetAmount ?? Util.ReadLine<decimal>("Bet amount (hand pot)?"));
				}
				else
				{
					// WHAT TO DO HERE??
				}
			}
		}
	}

	public Session(Players players, int minRoundsForCall = 4, decimal minBet = 10m, decimal? bombOutFixedPenalty = null, decimal? autoBetAmount = null)
	{
		MinRoundsForCall = minRoundsForCall;
		AutoBetAmount = autoBetAmount;
		MinBet = minBet;
		BombOutPenaltyStrategy = bombOutFixedPenalty == null ? BombOutPenaltyStrategy.HandPot : BombOutPenaltyStrategy.FixedAgreement;
		Players = players;
		FixedPenalty = bombOutFixedPenalty;
		Dealer = new Dealer(players);
		Sabacc = new Pot(players);
		Hand = new Pot(players);
	}
}


//  interference field - to prevent said card from being Shifted if a Sabacc Shift occurred
class VisibleProtectedField
{
	public Dictionary<Player, List<Card>> VisibleCards { get; }

	private Players Players { get; }

	public VisibleProtectedField(Players players)
	{
		VisibleCards = new();
		Players = players;
		Initialize();
	}

	private void Initialize()
	{
		foreach (var player in Players)
		{
			VisibleCards[player] = new List<Card>();
		}
	}

	public void Empty()
	{
		foreach (var player in VisibleCards.Keys)
		{
			VisibleCards[player].Clear();
		}
	}

	public void Protect(Card card)
	{
		if (card.Location is Hand h)
		{
			card.State = CardState.InProtectedField;
			VisibleCards[h.Player].Add(card);
		}
	}

	public void UnProtect(Card card)
	{
		if (card.Location is Hand h)
		{
			VisibleCards[h.Player].Remove(card);
			card.State = CardState.InHand;
		}
	}
}

class Dealer
{
	public Deck Deck { get; } = new();
	public Dice Dice { get; } = new();

	public VisibleProtectedField Field { get; }

	public Players Players { get; }

	public Dealer(Players players)
	{
		Players = players;
		Field = new VisibleProtectedField(players);
	}

	public void Deal(Player player)
	{
		player.Hand.Recieve(Draw());
	}

	public Card Draw()
	{
		Card drawn = Deck.Cards[0];
		Deck.Cards.RemoveAt(0);
		return drawn;
	}

	internal void ShuffleDeck()
	{
		Deck.Shuffle();
	}

	internal void DealHands()
	{
		foreach (var player in Players)
		{
			Deal(player);
		}

		foreach (var player in Players)
		{
			Deal(player);
		}
	}

	internal void Play(Player player)
	{
		// Players cannot discard cards
		// draw one card from the deck
		// trade a card from their hand for a card from the deck
		// stand

		PlayerTurnActions(player);

		// Add 1 and Remove 1 card from the Field per turn
		PlayerCardActions(player);
	}

	public void Protect(Card card)
	{
		Field.Protect(card);
	}

	internal void SabaccShift()
	{
		// It is only during this round that players can place cards into the Disruptor Field to prevent them from being Shifted should a Shift occur. 
		// Players do this simply by placing their cards face-up in front of them. 

		// Players may only place one card per turn in the disruptor field, but may have as many cards total in the Field as they desire. 
		// The downside is that they may only remove one card per turn from the Field as well. 
		// Players may do both, though, remove one card from and place one card into the Field per turn.
		foreach (var player in Players)
		{
			PlayerCardActions(player);
		}
	}

	private void PlayerTurnActions(Player player)
	{
		var choice = Util.ReadLine<PlayChoice>("Draw (1), Trade (2) or Stand (3) ?");

		if (choice == PlayChoice.Draw)
		{
			Deal(player);
		}
		else if (choice == PlayChoice.Trade)
		{
			int value = Util.ReadLine<int>($"Card from hand to Trade? ({string.Join(", ", player.Tradable)})");

			Card card = player.Hand.Cards.Find(x => x.State == CardState.InHand && x.Value == value);
			player.Hand.Cards.Remove(card);
			Deal(player);
			Deck.AddToDeck(card);
		}
		else if (choice == PlayChoice.Stand)
		{
			// Do nothing?
		}
	}

	private void PlayerCardActions(Player player)
	{
		Console.WriteLine($"Your hand: {player.Hand}");

		int? protect = Util.ReadLine<int?>($"Enter card to place into field (or blank for none): {player.Hand.Unprotected}");

		if (protect.HasValue)
		{
			var cardToProtect = player.Hand.Cards.First(c => c.State == CardState.InHand && c.Value == protect);
			Field.Protect(cardToProtect);
		}

		int? unprotect = Util.ReadLine<int?>($"Enter card to remove from field (or blank for none): {player.Hand.Protected}");

		if (unprotect.HasValue)
		{
			var cardToUnProtect = player.Hand.Cards.First(c => c.State == CardState.InProtectedField && c.Value == unprotect);
			Field.UnProtect(cardToUnProtect);
		}

	}
}

class Player
{
	public string Name { get; }

	public decimal Credits { get; private set; }

	public Hand Hand { get; }

	public int TotalSum => Hand.Cards.Select(x => x.Value).Sum();

	public bool CanCall => HasAnIdiotArray() || HasPositiveSabacc || HasNegativeSabacc;
	public bool HasNegativeSabacc => TotalSum == -21;
	public bool HasPositiveSabacc => TotalSum == 21;

	public bool HasAnIdiotArray()
	{
		var cardsToCheck = Hand.Cards.Where(c => c.State == CardState.InHand).Select(x => x.Value);
		var specialValues = cardsToCheck.OfType<SpecialValue>();
		var suitValues = cardsToCheck.OfType<SuitValue>();

		return specialValues.Contains(SpecialValue.TheIdiot) && suitValues.Contains(SuitValue.Two) && suitValues.Contains(SuitValue.Three);
	}

	public IEnumerable<Card> Tradable => Hand.Cards.Where(c => c.State == CardState.InHand);

	public Player(string name, decimal credits)
	{
		Name = name;
		Credits = credits;
		Hand = new Hand(this);
	}

	public void Bet(Pot pot, decimal amount)
	{
		Credits -= amount;
		pot.Add(this, amount);
	}

	public void DeclareTotal()
	{
		Console.WriteLine($"{Name} hand total: {Hand.Total}");
	}
}

#region Enums

enum BombOutPenaltyStrategy { HandPot, FixedAgreement }
enum PlayChoice { Draw = 1, Trade, Stand }
enum CardState { InDeck, InHand, InProtectedField }
enum PotType { TheHand, TrueSabacc }
enum Suit { Flasks, Sabers, Staves, Coins };

enum SuitValue
{
	One = 1,
	Two = 2,
	Three = 3,
	Four = 4,
	Five = 5,
	Six = 6,
	Seven = 7,
	Eight = 8,
	Nine = 9,
	Ten = 10,
	Eleven = 11,
	Commander = 12,
	Mistress = 13,
	Master = 14,
	Ace = 15
}

enum SpecialValue
{
	TheIdiot = 0,
	TheQueenOfAirAndDarkness = -2,
	Endurance = -8,
	Balance = -11,
	Demise = -13,
	Moderation = -14,
	TheEvilOne = -15,
	TheStar = -17
}

#endregion

class Hand : ICardOwner
{
	public List<Card> Cards { get; private set; }
	public Player Player { get; }

	public int Total => Cards.Sum(card => card.Value);

	public string Protected => string.Join(", ", Cards.Where(c => c.State == CardState.InProtectedField));
	public string Unprotected => string.Join(", ", Cards.Where(c => c.State == CardState.InHand));

	public string Name => $"{Player.Name}'s ";

	public Hand(Player player)
	{
		Cards = new();
		Player = player;
	}

	public void Recieve(Card card)
	{
		card.Location = this;
		card.State = CardState.InHand;
		Cards.Add(card);
	}

	public override string ToString()
	{
		return string.Join(", ", Cards.OrderBy(x => x.State).Select(c => $"{c.Value}({c.State})"));
	}
}

class Dice
{
	private List<Die> dice;

	public void Roll() => dice.ForEach(die => die.Roll());

	public bool IsSabaccShift() => dice[0].Equals(dice[1]);

	public Dice()
	{
		dice = new List<Die>(2) { new(), new() };
	}

	public class Die : IEquatable<Die>
	{
		public Sides Side { get; private set; }

		public void Roll()
		{
			Side = (Sides)((Sides[])Enum.GetValues(typeof(Sides))).OrderBy(x => Guid.NewGuid()).First();
		}

		public bool Equals(Die other) => this.Side.Equals(other.Side);

		public Die() => Roll();

		public enum Sides
		{
			One = 1,
			Two,
			Three,
			Four,
			Five,
			Six
		};
	}
}

class Pot
{
	PotType Type { get; }
	Players Players { get; }
	Dictionary<Player, decimal> Contributions { get; } = new();

	public Pot(Players players)
	{
		Players = players;

		Initialize();
	}

	private void Initialize()
	{
		foreach (var player in Players)
		{
			Contributions[player] = 0.0m;
		}
	}

	public void Add(Player player, decimal amount)
	{
		Contributions[player] += amount;
	}
}

class Deck : ICardOwner
{
	public List<Card> Cards { get; private set; }

	public string Name => "The dealers Deck";

	public Deck()
	{
		Cards = new List<Card>();

		foreach (var suit in (Suit[])Enum.GetValues(typeof(Suit)))
		{
			foreach (var value in (SuitValue[])Enum.GetValues(typeof(SuitValue)))
			{
				Cards.Add(new SuitCard(suit, value));
			}
		}

		foreach (var specialValue in (SpecialValue[])Enum.GetValues(typeof(SpecialValue)))
		{
			Cards.Add(new SpecialCard(specialValue));
			Cards.Add(new SpecialCard(specialValue));
		}
	}

	public void Shuffle()
	{
		Cards = new List<Card>(Cards.OrderBy(x => Guid.NewGuid()));
	}

	public void AddToDeck(Card card)
	{
		Cards.Add(card);
	}
}

interface ICard
{
	public int Value { get; }
	public string Name { get; }
	public string Type { get; }
	public ICardOwner Location { get; }
}

interface ICardOwner
{
	public string Name { get; }
}

class SuitCard : Card
{
	public Suit Suit { get; }

	public SuitValue SuitValue { get; }

	public override int Value => (int)SuitValue;

	public override string Name => Enum.GetName(typeof(Suit), Suit);

	public override string Type => nameof(Suit);

	public SuitCard(Suit suit, SuitValue value)
	{
		Suit = suit;
		SuitValue = value;
	}
}

class SpecialCard : Card, ICloneable
{
	public SpecialValue SpecialValue { get; }

	public SpecialCard(SpecialValue specialValue)
	{
		SpecialValue = specialValue;
	}

	public override int Value => (int)SpecialValue;

	public override string Name => Enum.GetName(typeof(SpecialValue), SpecialValue);

	public override string Type => nameof(SpecialCard);

	public object Clone()
	{
		return new SpecialCard(SpecialValue);
	}
}

abstract class Card : ICard, IEquatable<Card>
{
	public abstract int Value { get; }

	public abstract string Name { get; }

	public abstract string Type { get; }

	public ICardOwner Location { get; set; }

	public CardState State { get; set; } = CardState.InDeck;

	public bool Equals(Card other)
	{
		return object.ReferenceEquals(this, other);
	}

	public override string ToString()
	{
		return $"{Value}";
	}
}