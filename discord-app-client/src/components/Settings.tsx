import styles from "../css/Settings.module.css";

interface SettingsProps {
  onLogout: () => void;
}

export default function Settings({ onLogout }: SettingsProps) {
  return (
    <div className={styles.settingsContainer}>
      <h2 className={styles.title}>User Settings</h2>

      <div className={styles.settingsContent}>
        <h3>My Account</h3>
      </div>

      <button
        type="button"
        className={styles.logoutButton}
        onClick={onLogout}
      >
        Log Out
      </button>
    </div>
  );
}