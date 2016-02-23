namespace Infocom.TimeManager.Core.DomainModel
{
    public abstract class DomainObject
    {
        #region Properties

        public virtual long ID { get; internal protected set; }

        #endregion

        #region Public Methods

        public virtual bool Equals(DomainObject other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other.ID == 0)
            {
                return false; // object not persisted yet
            }

            return other.ID == this.ID;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is DomainObject)
            {
                return this.Equals((DomainObject)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public virtual bool IsTransient()
        {
            return this.ID == 0;
        }

        #endregion
    }
}