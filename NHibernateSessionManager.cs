using NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;

namespace MezaIT.NHibernate
{
    /// <summary>
    /// NHibernate session manager
    /// </summary>
    public class NHibernateSessionManager
    {
        private static ISessionFactory _sessionFactory;

        #region Private Properties

        /// <summary>
        /// NHibernate session factory
        /// </summary>
        public static ISessionFactory SessionFactory
        {
            get
            {
                // Build the session factory from the configuration
                return _sessionFactory ?? (_sessionFactory = new Configuration().Configure().BuildSessionFactory());
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// NHibernate session
        /// </summary>
        public static ISession Session
        {
            get
            {
                ISession session = null;

                if (CurrentSessionContext.HasBind(SessionFactory))
                    session = SessionFactory.GetCurrentSession();

                if (session == null || !session.IsOpen)
                    session = SessionFactory.OpenSession();

                return session;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Bind the current session
        /// </summary>
        /// <param name="session"><see cref="ISession" /> object</param>
        public static void Bind(ISession session)
        {
            CurrentSessionContext.Bind(session);
        }

        /// <summary>
        /// Unbind the current session
        /// </summary>
        public static ISession Unbind()
        {
            return CurrentSessionContext.Unbind(SessionFactory);
        }

        #endregion
    }
}