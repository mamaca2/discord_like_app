
import { useEffect, useRef, useState } from "react";
import type { FormEvent } from "react";
import type { ServerDto } from "../../services/serverService";
import styles from "../css/ServerList.module.css";

interface ServerListProps {
  servers: ServerDto[];
  activeServerId: number | null;
  activeBar: "friends" | "server" | "search";
  onSelectFriends: () => void;
  onSelectServer: (serverId: number) => void;
  onSettingsClick: () => void;
  onAddServer: (name: string, image: File | null) => void;
}

export function ServerList({
  servers,
  activeServerId,
  activeBar,
  onSelectFriends,
  onSelectServer,
  onSettingsClick,
  onAddServer
}: ServerListProps) {
  const [showCreateServer, setShowCreateServer] = useState(false);
  const [serverName, setServerName] = useState("");
  const [serverImage, setServerImage] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(null);

  const fileInputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (!serverImage) {
      setImagePreview(null);
      return;
    }

    const url = URL.createObjectURL(serverImage);
    setImagePreview(url);

    return () => URL.revokeObjectURL(url);
  }, [serverImage]);

  const handleUseDefault = () => {
    setServerImage(null);

    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const handleCreateServer = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (!serverName.trim()) return;

    onAddServer(serverName.trim(), serverImage);

    setServerName("");
    handleUseDefault();
    setShowCreateServer(false);
  };

  return (
    <nav className={styles.serverSidebar}>
      <button
        type="button"
        className={`${styles.friendsIcon} ${
          activeBar !== "server" ? styles.activeTab : ""
        }`}
        onClick={onSelectFriends}
        title="Direct Messages & Friends"
      >
        <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
          <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z" />
        </svg>
      </button>

      <div className={styles.separator} />

      {servers.map(server => {
        const isActive =
          activeBar === "server" && activeServerId === server.id;

        return (
          <button
            type="button"
            key={server.id}
            className={`${styles.serverIcon} ${
              isActive ? styles.activeServer : ""
            }`}
            onClick={() => onSelectServer(server.id)}
            title={server.name}
          >
            {server.image ? (
              <img
                src={server.image}
                alt={server.name}
                className={styles.serverImage}
              />
            ) : (
              <svg
                width="24"
                height="24"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
                strokeLinecap="round"
                strokeLinejoin="round"
              >
                <rect x="3" y="3" width="18" height="8" rx="2" />
                <rect x="3" y="13" width="18" height="8" rx="2" />
                <circle cx="7" cy="7" r="1" fill="currentColor" />
                <circle cx="7" cy="17" r="1" fill="currentColor" />
              </svg>
            )}
          </button>
        );
      })}

      <div className={styles.addServerWrapper}>
        <button
          type="button"
          className={`${styles.serverIcon} ${styles.addServerIcon}`}
          onClick={() => setShowCreateServer(!showCreateServer)}
          title="Add Server"
        >
          <svg
            width="24"
            height="24"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeLinecap="round"
          >
            <path d="M12 5v14M5 12h14" />
          </svg>
        </button>

        {showCreateServer && (
          <form
            className={styles.createServerPopup}
            onSubmit={handleCreateServer}
          >
            <h3>Create a Server</h3>

            <div className={styles.imageSection}>
              <div className={styles.imagePreview}>
                {imagePreview ? (
                  <img
                    src={imagePreview}
                    alt="Server icon preview"
                  />
                ) : (
                  <svg
                    width="36"
                    height="36"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="1.8"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  >
                    <rect x="3" y="3" width="18" height="8" rx="2" />
                    <rect x="3" y="13" width="18" height="8" rx="2" />
                    <circle cx="7" cy="7" r="1" fill="currentColor" />
                    <circle cx="7" cy="17" r="1" fill="currentColor" />
                  </svg>
                )}
              </div>

              <label className={styles.uploadButton}>
                Upload Image
                <input
                  ref={fileInputRef}
                  type="file"
                  accept="image/*"
                  hidden
                  onChange={event =>
                    setServerImage(event.target.files?.[0] ?? null)
                  }
                />
              </label>

              <button
                type="button"
                className={styles.defaultButton}
                onClick={handleUseDefault}
              >
                Use Default
              </button>
            </div>

            <label htmlFor="serverName">SERVER NAME</label>

            <input
              id="serverName"
              type="text"
              value={serverName}
              onChange={event => setServerName(event.target.value)}
              placeholder="Enter server name"
              autoFocus
            />

            <div className={styles.popupActions}>
              <button
                type="button"
                onClick={() => setShowCreateServer(false)}
              >
                Cancel
              </button>

              <button
                type="submit"
                disabled={!serverName.trim()}
              >
                Create
              </button>
            </div>
          </form>
        )}
      </div>

      <button
        type="button"
        className={styles.settingsIcon}
        onClick={onSettingsClick}
        title="Settings"
      >
        <svg
          width="24"
          height="24"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="2"
          strokeLinecap="round"
          strokeLinejoin="round"
        >
          <path d="M12 8a4 4 0 1 0 0 8 4 4 0 0 0 0-8Z" />
          <path d="M19.4 15a1.7 1.7 0 0 0 .34 1.88l.06.06-2.86 2.86-.06-.06A1.7 1.7 0 0 0 15 19.4a1.7 1.7 0 0 0-1.4 1.6V21h-4v-.08A1.7 1.7 0 0 0 8 19.4a1.7 1.7 0 0 0-1.88.34l-.06.06-2.86-2.86.06-.06A1.7 1.7 0 0 0 19.4 8a1.7 1.7 0 0 0-.34-1.88L3.2 6.06 6.06 3.2l.06.06A1.7 1.7 0 0 0 8 3.6 1.7 1.7 0 0 0 9.4 2H13.6A1.7 1.7 0 0 0 15 3.6a1.7 1.7 0 0 0 1.88-.34l.06-.06 2.86 2.86-.06.06A1.7 1.7 0 0 0 19.4 8 1.7 1.7 0 0 0 21 9.4V13.6A1.7 1.7 0 0 0 19.4 15Z" />
        </svg>
      </button>
    </nav>
  );
}
