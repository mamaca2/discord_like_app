import type { FriendRequestDto } from "../../services/friendRequestService";
import styles from "../css/FriendRequests.module.css";

interface FriendRequestsProps {
  incomingRequests: FriendRequestDto[];
  outgoingRequests: FriendRequestDto[];
  onAccept: (requestId: number) => void;
  onDecline: (requestId: number) => void;
}

export function FriendRequests({
  incomingRequests,
  outgoingRequests,
  onAccept,
  onDecline,
}: FriendRequestsProps) {
  return (
    <div className={styles.container}>
      <h2 className={styles.title}>Friend Requests</h2>

      <h3 className={styles.sectionTitle}>
        INCOMING — {incomingRequests.length}
      </h3>

      {incomingRequests.length === 0 && (
        <p className={styles.emptyMessage}>No incoming requests</p>
      )}

      {incomingRequests.map((request) => (
        <div key={request.id} className={styles.requestRow}>
          {request.senderImage ? (
            <img
              className={styles.avatar}
              src={request.senderImage}
              alt={request.senderUserName}
            />
          ) : (
            <div className={styles.avatarPlaceholder}>
              {request.senderUserName.charAt(0).toUpperCase()}
            </div>
          )}

          <div className={styles.userInfo}>
            <span className={styles.username}>
              {request.senderUserName}
            </span>
            <span className={styles.tag}>
              #{request.senderTag}
            </span>
          </div>

          <div className={styles.actions}>
            <button
              type="button"
              className={styles.acceptButton}
              onClick={() => onAccept(request.id)}
              title="Accept"
            >
              ✓
            </button>

            <button
              type="button"
              className={styles.declineButton}
              onClick={() => onDecline(request.id)}
              title="Decline"
            >
              ✕
            </button>
          </div>
        </div>
      ))}

      <h3 className={styles.sectionTitle}>
        OUTGOING — {outgoingRequests.length}
      </h3>

      {outgoingRequests.length === 0 && (
        <p className={styles.emptyMessage}>No outgoing requests</p>
      )}

      {outgoingRequests.map((request) => (
        <div key={request.id} className={styles.requestRow}>
          {request.receiverImage ? (
            <img
              className={styles.avatar}
              src={request.receiverImage}
              alt={request.receiverUserName}
            />
          ) : (
            <div className={styles.avatarPlaceholder}>
              {request.receiverUserName.charAt(0).toUpperCase()}
            </div>
          )}

          <div className={styles.userInfo}>
            <span className={styles.username}>
              {request.receiverUserName}
            </span>
            <span className={styles.tag}>
              #{request.receiverTag}
            </span>
          </div>

          <span className={styles.pendingStatus}>
            Pending
          </span>
        </div>
      ))}
    </div>
  );
}