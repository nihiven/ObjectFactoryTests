using System;
using System.Collections.Generic;

namespace ObjectFactoryTests
{
    public static class ObjectFactory
    {
        public static object New(string identifier)
        {
            switch (identifier)
            {
                case ("CompOne"):
                    return new CompOne();
                case ("CompTwo"):
                    return new CompTwo("it's number two!");
                case ("CompThree"):
                    return new CompThree("if you see this, bad news...");
                default:
                    return null;
            }
        }
    }

    public static class ObjectManager
    {
        private static Dictionary<string, Object> objects = new Dictionary<string, object>(32);

        public static void Add(string identifier, Object obj)
        {
            // remove an old key,value pair if it exists
            if (objects.ContainsKey(identifier))
                objects.Remove(identifier);

            // add the new key,value pair to our list
            objects.Add(identifier, obj);
        }

        public static object Get(string identifier)
        {
            // make sure an object exists for identifier
            if (objects.ContainsKey(identifier))
                return objects[identifier]; // return our stored object instance
            else
                return null;
        }

        public static void Clear()
        {
            objects = new Dictionary<string, object>(32);
        }

        public static Dictionary<string, object>.KeyCollection ListIdentifiers()
        {
            // returns a list that can be iterated over
            return objects.Keys;
        }
    }

    public class CompOne : IUpdateable
    {
        public void Print()
        {
            // get the CoolData property from Comp2
            // we use the ObjectManager, so we don't need an explicit reference to Comp2
            CompTwo compTwo = (CompTwo)ObjectManager.Get("comp2");
            Console.WriteLine(compTwo.CoolData);
        }

        public void Update(bool status)
        {
            // this implements IUpdateable
            if (status)
                Console.WriteLine("comp1 checking in");
        }
    }

    public class CompTwo : IUpdateable
    {
        public string CoolData;

        public CompTwo(string coolData)
        {
            CoolData = coolData;
        }

        public void Print()
        {
            Console.WriteLine(CoolData);
        }

        public void Update(bool status)
        {
            // this implements IUpdateable
            if (status)
                Console.WriteLine("comp2 checking in");
        }
    }

    // a third Comp class that does not implement IUpdateable
    // despite being in the object list, this class won't have it's Update method called 
    // because IUpdateable is required to invoke Update().
    public class CompThree
    {
        public string CoolData;

        public CompThree(string coolData)
        {
            CoolData = coolData;
        }

        public void Print()
        {
            Console.WriteLine(CoolData);
        }

        public void Update(bool status)
        {
            // this implements IUpdateable
            if (status)
                Console.WriteLine("comp3 checking in");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // create and add components to the manager
            ObjectManager.Add("comp1", ObjectFactory.New("CompOne"));
            ObjectManager.Add("comp2", ObjectFactory.New("CompTwo"));
            ObjectManager.Add("comp3", ObjectFactory.New("CompThree"));

            // create a local CompOne. get an Object from the ObjectManger, cast it to CompOne
            // then call print, which is a method of CompOne
            CompOne c1 = (CompOne)ObjectManager.Get("comp1");
            c1.Print();

            // iterate over the list of identifiers in our objects list
            foreach (string obj in ObjectManager.ListIdentifiers())
            {
                // convert the object to IUpdateable
                // this allows us to call update on the object, because it is required by IUpdateable
                // notice that comp3 never checks in
                if (ObjectManager.Get(obj) is IUpdateable o)
                    o.Update(true);
            }

            // get two instances of Comp2 
            // shows that mulitple calls to the Manager with a given identifier yield the same object
            CompTwo comp2_1 = (CompTwo)ObjectManager.Get("comp2");
            CompTwo comp2_2 = (CompTwo)ObjectManager.Get("comp2");

            // change the member data in once instance
            comp2_1.CoolData = "not so cool";

            // print the changed data in another instance
            comp2_2.Print();
        }
    }
}
