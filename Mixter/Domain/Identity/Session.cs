﻿using System;
using Mixter.Domain.Identity.Events;

namespace Mixter.Domain.Identity
{
    public class Session
    {
        private readonly DecisionProjection _projection = new DecisionProjection();

        public Session(params IDomainEvent[] events)
        {
            foreach (var @event in events)
            {
                _projection.Apply(@event);
            }
        }

        public static void LogUser(IEventPublisher eventPublisher, UserId userId)
        {
            var id = SessionId.Generate();
            eventPublisher.Publish(new UserConnected(id, userId, DateTime.Now));
        }

        public void Logout(IEventPublisher eventPublisher)
        {
            eventPublisher.Publish(new UserDisconnected(_projection.Id, _projection.UserId));
        }

        private class DecisionProjection : DecisionProjectionBase
        {
            public SessionId Id { get; private set; }

            public UserId UserId { get; private set; }

            public DecisionProjection()
            {
                AddHandler<UserConnected>(When);
            }

            private void When(UserConnected evt)
            {
                Id = evt.SessionId;
                UserId = evt.UserId;
            }
        }
    }
}
