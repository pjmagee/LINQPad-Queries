<Query Kind="Program" />

void Main()
{
	var fourth = new Structure();
 
	var tangibles = new [] { new Structure(), new Structure(), new Structure(), fourth, new Structure() };
	var agent = new Agent(){ Tangibles = tangibles };
	
	foreach(ITangible item in agent.Tangibles)
	{
		// recorded
		if(item == fourth) break;
	}
}

public interface IHumanic
{
	Guid Id { get; set; }
	IEnumerable<IIntangible> Intangibles { get; }
	IEnumerable<ITangible> Tangibles { get; }
} 
 
public interface IIntangible 
{
	Guid Id { get; set; }
}

public interface ITangible 
{
	Guid Id { get; set; }	
	IEnumerable<ICommand> Commands { get; }
}
 
public abstract class Tangible : ITangible
{
	public Guid Id { get; set; }
	
	public Tangible()
	{
		Id = Guid.NewGuid();		
	}
	
	public IEnumerable<ICommand> Commands 
	{
		get 
		{
			return GetCommands();
		}
	}
	
	public abstract IEnumerable<ICommand> GetCommands();
}
 
public class Structure : Tangible
{
	public override IEnumerable<ICommand> GetCommands()
	{
		yield break;
	}
}
 
public interface ICommand : IIntangible
{
	void Execute();
}
 
public class Command : ICommand
{
	public Guid Id { get; set; }
 
	public void Execute()
	{	
		Console.WriteLine("execute command & perform required action");
	}
}
 
public class Agent : IHumanic
{
	public Guid Id { get; set; }
	public IEnumerable<ITangible> Tangibles { get; set; }
	public IEnumerable<IIntangible> Intangibles { get; set; }
	
	public Agent()
	{
		Id = Guid.NewGuid();
	}
}
 
public class TangibleCollection : IEnumerable<ITangible>, IEnumerator<ITangible>
{
   private IEnumerator<ITangible> _enumerator; 
   private IEnumerable<ITangible> _tangibles;
 
   public TangibleCollection(IEnumerable<ITangible> tangibles)
   {
   	   _tangibles = tangibles;
   }
  
   public IEnumerator<ITangible> GetEnumerator()
   {
   		return _enumerator ?? (_enumerator = _tangibles.GetEnumerator());
   }
   
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
	
	public void Dispose()
    {
        _enumerator.Dispose();
    }
 
   public bool MoveNext()
   {   	   
       // Energy depleted here
       return _enumerator.MoveNext();
   }
 
   public void Reset()
   {
       _enumerator = _tangibles.GetEnumerator();
   }
 
   public ITangible Current
   {
       get
       {
           return _enumerator.Current;
       }
   }
 
   object IEnumerator.Current
   {
       get
       {
           return _enumerator.Current;
       }
   }
}