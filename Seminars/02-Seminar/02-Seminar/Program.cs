namespace _02_Seminar;

public class GeneralPersonInfo
{
    public string Name { get; }
    public string Surname { get; }
    public GeneralPersonInfo(string name, string surname)
    {
        Name = name;
        Surname = surname;
    }
    public override string ToString()
    {
        return $"{Surname} {Name}";
    }
}

public class ContactPersonInfo
{
    public string PhoneNumber { get; }
    public string Email { get; }
    public ContactPersonInfo(string phoneNumber, string email)
    {
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public override string ToString()
    {
        return $"PhoneNumber: {PhoneNumber}, Email: {Email}";
    }
}

public interface IPerson
{
    public int Id { get; }
    public ContactPersonInfo ContactInfo { get; }
    public GeneralPersonInfo GeneralInfo { get; }
}

public class Client : IPerson
{
    public int Id { get; }
    public ContactPersonInfo ContactInfo { get; }
    public GeneralPersonInfo GeneralInfo { get; }
    
    public Client(int clientID, string name, string surname, string phoneNumber, string email)
    {
        Id = clientID;
        ContactInfo = new ContactPersonInfo(phoneNumber, email);
        GeneralInfo = new GeneralPersonInfo(name, surname);
    }
    public override string ToString()
    {
        return $"Client {GeneralInfo} with ID = {Id}";
    }
}

public class Broker : IPerson
{
    public int Id { get; }
    public ContactPersonInfo ContactInfo { get; }
    public GeneralPersonInfo GeneralInfo { get; }

    public Broker(int clientID, string name, string surname, string phoneNumber, string email)
    {
        Id = clientID;
        ContactInfo = new ContactPersonInfo(phoneNumber, email);
        GeneralInfo = new GeneralPersonInfo(name, surname);
    }

    public override string ToString()
    {
        return $"Broker {GeneralInfo} with ID = {Id}";
    }
}

public interface IProperty
{
    public string PropertyType { get; }
    public bool IsMovable {  get; }
    public int MarketPrice { get; }
    public int CoveragePrice { get; }
}

public class Car : IProperty
{
    public string PropertyType { get; }
    public bool IsMovable { get; }
    public int MarketPrice { get;  }
    public int CoveragePrice { get; }
    public string CarModel { get;  }
    public string LicensePlate {  get; }


    public Car(string propertyType, bool isMovable, int marketPrice, int coveragePrice, string carModel, string licensePlate) {
        PropertyType = propertyType;
        IsMovable = isMovable;
        MarketPrice = marketPrice;
        CoveragePrice = coveragePrice;
        CarModel = carModel;
        LicensePlate = licensePlate;
    }
}

public class Flat : IProperty
{
    public string PropertyType { get; }
    public bool IsMovable { get; }
    public int MarketPrice { get; }
    public int CoveragePrice { get; }
    public string Address { get; }
    public double Area { get; }

    public Flat(string propertyType, bool isMovable, int marketPrice, int coveragePrice, string address, double area)
    {
        PropertyType = propertyType;
        IsMovable = isMovable;
        MarketPrice = marketPrice;
        CoveragePrice = coveragePrice;
        Address = address;
        Area = area;
    }
}

public class InsuranceEvent
{
    public DateTime EventDate { get;  }
    public string Description { get; }

    public InsuranceEvent(DateTime eventDate, string description)
    {
        EventDate = eventDate;
        Description = description;
    }
}

public class InsurancePolicy
{
    public Client Client { get;}
    public Broker Broker { get; }
    public IProperty InsuredObject { get;  }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public decimal CoverageAmount { get; }

    public InsurancePolicy(Client client, Broker broker, IProperty insuredObject, DateTime startDate, DateTime endDate)
    {
        Client = client;
        Broker = broker;
        InsuredObject = insuredObject;
        StartDate = startDate;
        EndDate = endDate;
    }
    public void ProcessInsuranceEvent(InsuranceEvent insuranceEvent)
    {
        var payValue = InsuredObject.CoveragePrice;
        Console.WriteLine("===========================");
        Console.WriteLine($"Insurance event processed for {Client}");
        Console.WriteLine($"The {Broker}");
        Console.WriteLine($"Event description: {insuranceEvent.Description}");
        Console.WriteLine($"Coverage amount: {payValue}");
        Console.WriteLine("===========================");
    }
}

internal class Program
{

    static void Main(string[] args)
    {
        var client = new Client(1, "Ivan","Porgin", "+79308097097", "somi.saaw@mail.ru");
        var broker = new Broker(1, "Petr", "Hier", "+79308997012", "som.saw@mail.ru");

        var car = new Car("Toyota Camry", true, 10000, 5000, "Mazda", "A123BC");

        var policy = new InsurancePolicy(client, broker, car, DateTime.Now, DateTime.Now.AddYears(1));

        var insuranceEvent = new InsuranceEvent(DateTime.Now, "ДТП на перекрестке");
        policy.ProcessInsuranceEvent(insuranceEvent);

        var apartment = new Flat("flat",false, 20000, 10000, "ул. Ленина, д. 10", 75.5);

        var apartmentPolicy = new InsurancePolicy(client, broker, apartment, DateTime.Now, DateTime.Now.AddYears(1));

        var apartmentEvent = new InsuranceEvent(DateTime.Now, "Затопление квартиры");
        apartmentPolicy.ProcessInsuranceEvent(apartmentEvent);
    }
}
