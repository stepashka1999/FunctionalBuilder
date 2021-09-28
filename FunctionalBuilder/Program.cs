using System;
using System.Collections.Generic;
using System.Linq;

namespace FunctionalBuilder
{
    public class Person
    {
        public string Name, Position;

        public override string ToString()
        {
            return $"{Name} works as a {Position}";
        }
    }

    public abstract class FunctionalBuilder<TSubject, TSelf>
        where TSelf : FunctionalBuilder<TSubject, TSelf>
        where TSubject : new()
    {
        private IList<Func<TSubject, TSubject>> _actions = new List<Func<TSubject, TSubject>>();

        public TSelf AddAction(Action<TSubject> action) 
        {
            _actions.Add(p =>
            {
                action(p);
                return p;
            });

            return (TSelf)this;
        }

        public TSubject Build() => _actions.Aggregate(new TSubject(), (p, f) => f(p));
    }

    public sealed class PersonBuilder : FunctionalBuilder<Person, PersonBuilder>
    {
        public PersonBuilder Called(string name) => AddAction(p => p.Name = name);
    }

    //И вот мы уже спокойно можем расширять билдер с помощью методов расширения)
    public static class PersonBuilderExtentions
    {
        public static PersonBuilder WorksAsA(this PersonBuilder builder, string position)
        {
            builder.AddAction(p => p.Position = position);
            return builder;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new PersonBuilder();

            var p = builder
                        .Called("Viktor")
                        .WorksAsA("Programmer")
                        .Build();

            Console.WriteLine(p);
        }
    }
}
