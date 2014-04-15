using System;
using NHibernate;

namespace MezaIT.NHibernate
{
    /// <summary>
    /// NHibernate unit of work implementation
    /// </summary>
    public class NHibernateUnitOfWork : IDisposable
    {
        private readonly bool _commit = true;
        private readonly ISession _session;

        #region Constructors

        /// <summary>
        /// Open a session and start a new transaction
        /// </summary>
        public NHibernateUnitOfWork()
        {
            _session = NHibernateSessionManager.Session;

            _session.BeginTransaction();

            NHibernateSessionManager.Bind(_session);
        }

        /// <summary>
        /// Open a session and start a new transaction
        /// </summary>
        /// <param name="commit">False, if the transaction should not commit automatically</param>
        public NHibernateUnitOfWork(bool commit) : this()
        {
            _commit = commit;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Commit the unit of work
        /// </summary>
        public NHibernateUnitOfWork Commit()
        {
            var transaction = _session.Transaction;

            try
            {
                if (transaction.IsActive)
                    transaction.Commit();
            }
            catch
            {
                transaction.Rollback();

                throw;
            }
            finally
            {
                transaction.Dispose();
            }

            return this;
        }

        public void Dispose()
        {
            if (_commit)
                Commit();

            NHibernateSessionManager.Unbind().Dispose();
        }

        /// <summary>
        /// Rollback the unit of work
        /// </summary>
        public NHibernateUnitOfWork Rollback()
        {
            var transaction = _session.Transaction;

            if (transaction.IsActive)
                transaction.Rollback();

            return this;
        }

        #endregion
    }
}