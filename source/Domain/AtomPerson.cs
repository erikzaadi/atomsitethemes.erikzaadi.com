/* Copyright 2008-10 Jarrett Vance
 * see http://www.codeplex.com/blogsvc/license */
namespace AtomSite.Domain
{
	using System;
	using System.Xml.Linq;

	/// <summary>
	/// Describes a person, corporation, or similar entity. It has one required element,
	/// name, and two optional elements: url, email.
	/// </summary>
	public class AtomPerson : XmlBase
	{
		public AtomPerson(XName name) : this(new XElement(name)) { }
		public AtomPerson() : this(new XElement(Atom.AtomNs + "person")) { }
		public AtomPerson(XElement xml) : base(xml) { }

		/// <summary>
		/// Gets the type of person, usually "author" or "contributor"
		/// </summary>
		public string Type
		{
			get { return Xml.Name.LocalName; }
		}

		/// <summary>
		/// Gets or sets the id of the person.  This is an extension
		/// </summary>
		public string Id
		{
			get { return GetProperty<string>(Atom.SvcNs + "id"); }
			set { SetProperty<string>(Atom.SvcNs + "id", value); }
		}

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return GetValue<string>(Atom.AtomNs + "name"); }
			set { SetValue<string>(Atom.AtomNs + "name", value); }
		}

		/// <summary>
		/// Gets or sets the homepage.
		/// </summary>
		/// <value>The homepage.</value>
		public Uri Uri
		{
			get { return GetUri(Atom.AtomNs + "uri"); }
			set { SetUri(Atom.AtomNs + "uri", value); }
		}

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>The email.</value>
		public string Email
		{
			get { return GetValue<string>(Atom.AtomNs + "email"); }
			set { SetValue<string>(Atom.AtomNs + "email", value); }
		}

		public override string ToString()
		{
			return !string.IsNullOrEmpty(Name) ? Name
				: !string.IsNullOrEmpty(Email) ? Email
				: string.Empty;
		}

		/// <summary>
		/// Returns true if this object is equal to <c>obj</c>.
		/// </summary>
		/// <param name="obj">Object you wish to compare to.</param>
		/// <returns>true if this object is equal to <c>obj</c>.</returns>
		public override bool Equals(object obj)
		{
			if (obj != null && obj.GetType().Equals(this.GetType()))
			{
				AtomPerson other = obj as AtomPerson;
				if ((object)other != null)
				{
					if (other.Id != null && Id != null) return other.Id == Id;
					return other.Name == Name & other.Uri == Uri & other.Email == Email;
					//TODO: extensions?
				}
			}
			return base.Equals(obj);
		}
		public override int GetHashCode()
		{
			if (Id != null) return Id.GetHashCode();
			if (Name != null && Uri != null && Email != null)
				return Name.GetHashCode() ^ Uri.GetHashCode() ^ Email.GetHashCode();
			else if (Name != null && Uri != null)
				return Name.GetHashCode() ^ Uri.GetHashCode();
			else if (Name != null && Email != null)
				return Name.GetHashCode() ^ Email.GetHashCode();
			else if (Name != null)
				return Name.GetHashCode();
			else
				return base.GetHashCode();
		}
	}
	public class AtomAuthor : AtomPerson
	{
		public AtomAuthor() : base(Atom.AtomNs + "author") { }
    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
	}
	public class AtomContributor : AtomPerson
	{
    public AtomContributor() : base(Atom.AtomNs + "contributor") { }
    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
	}
}
