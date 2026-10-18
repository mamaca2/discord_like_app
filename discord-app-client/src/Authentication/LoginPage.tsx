import { useState } from "react";
import { loginUser } from "../services/authService";
import styles from "./AuthenticationPage.module.css";

interface AuthenticationPageProps {
  onLoginSuccess: (token: string) => void;
  onGoToRegister: () => void;
}


export default function LoginPage({   onLoginSuccess, onGoToRegister }: AuthenticationPageProps) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setIsLoading(true);

    try {
      const token = await loginUser({ email, password });
      onLoginSuccess(token);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : "Login failed";
      setError(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className={styles.authContainer}>
      <h2 className={styles.title}>Welcome Back</h2>
      {error && <p className={styles.errorMessage}>{error}</p>}

      <form onSubmit={handleSubmit} className={styles.form}>
        <div className={styles.fieldGroup}>
          <label>Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            disabled={isLoading}
            autoComplete="email"
            className={styles.input}
          />
        </div>

        <div className={styles.fieldGroup}>
          <label>Password</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            disabled={isLoading}
            autoComplete="current-password"
            className={styles.input}
          />
        </div>

        <button type="submit" disabled={isLoading} className={styles.submitButton}>
          {isLoading ? "Logging in..." : "Log In"}
        </button>
        <button
          type="button"
          onClick={onGoToRegister}
          disabled={isLoading}
          className={styles.registerButton}
        >
          Register
        </button>
      </form>
    </div>
  );
}