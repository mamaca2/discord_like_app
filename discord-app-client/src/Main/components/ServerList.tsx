import type { ServerDto } from "../../services/serverService";
import styles from "../MainPage.module.css";

interface ServerListProps {
  servers: ServerDto[];
  activeServerId: number | null;
  isFriendsActive: boolean;
  onSelectFriends: () => void;
  onSelectServer: (serverId: number) => void;
  onAddServer?: () => void;
  onLogout?: () => void;
}

export function ServerList({
  servers,
  activeServerId,
  isFriendsActive,
  onSelectFriends,
  onSelectServer,
  onAddServer,
  onLogout,
}: ServerListProps) {
  return (
    <nav className={styles.serverSidebar}>
      <button
        type="button"
        className={`${styles.friendsIcon} ${isFriendsActive ? styles.activeTab : ""}`}
        onClick={onSelectFriends}
        title="Direct Messages & Friends"
      >
        <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
          <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z" />
        </svg>
      </button>

      <div className={styles.separator} />

      {servers.map((server) => {
        const isActive = !isFriendsActive && activeServerId === server.id;
        return (
          <button
            type="button"
            key={server.id}
            className={`${styles.serverIcon} ${isActive ? styles.activeServer : ""}`}
            onClick={() => onSelectServer(server.id)}
            title={server.name}
          >
            {server.image ? (
              <img src={server.image} alt={server.name} className={styles.serverImage} />
            ) : (
              server.name.substring(0, 2).toUpperCase()
            )}
          </button>
        );
      })}

      <button type="button" className={styles.serverIcon} onClick={onAddServer} title="Add a server">
        +
      </button>

      <button type="button" className={styles.logoutButton} onClick={onLogout} title="Log out">
        ⎋
      </button>
    </nav>
  );
}