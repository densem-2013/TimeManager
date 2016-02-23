namespace Infocom.TimeManager.WebAccess.Models
{
    using System.Dynamic;

    public abstract class BaseModel
    {
        #region Properties

        public long ID { get; set; }

        #endregion

        #region Public Methods

        public virtual bool Initialized()
        {
            return this.ID != 0;
        }

        public virtual bool Equals(BaseModel other)
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

            if (obj is BaseModel)
            {
                return this.Equals((BaseModel)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        #endregion
    }

    public abstract class DynamicBaseModel : DynamicObject
    {
        #region Properties

        public long ID { get; set; }

        #endregion

        #region Public Methods

        public virtual bool Initialized()
        {
            return this.ID != 0;
        }

        public virtual bool Equals(BaseModel other)
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

            if (obj is BaseModel)
            {
                return this.Equals((BaseModel)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        #endregion
    }

}