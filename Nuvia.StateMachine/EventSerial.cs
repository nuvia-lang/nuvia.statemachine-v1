namespace Nuvia.StateMachine;

//https://stackoverflow.com/questions/10957528/structural-equality-in-f

//Major should be zero for a loooooong time! Idea is to only create a second serialized event store when we absolutely must.
//Keeping everything serialized in a single database table is just so much simpler. Simplifies querying etc.
//For backups, we'd do differential backups and also stream the events to multiple backups for redundancy,
//availbility etc.

//Serial number or id on the "serialized event store"
public record struct EventSerial(long Major, long Minor) : IComparable<EventSerial>
{

    // Result:
    //  < 0 : this instance less than other
    //  = 0 : this instance equivalent to other
    //  > 0 : this instance greater than other   
    public int CompareTo(EventSerial other)
    {
        //Compare the Major values of both instances.
        var result = this.Major.CompareTo(other.Major);

        //If the Major values are equal, compare the Minor values.
        if(result == 0)
        {
            result = this.Minor.CompareTo(other.Minor);
        }

        return result;
    }

    public static bool operator <(EventSerial left, EventSerial right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(EventSerial left, EventSerial right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(EventSerial left, EventSerial right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(EventSerial left, EventSerial right)
    {
        return left.CompareTo(right) >= 0;
    }
}
