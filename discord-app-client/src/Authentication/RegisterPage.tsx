import { useState } from "react";
import {
  startRegistration,
  verifyCode,
  completeRegistration,
} from "../services/authService";
import styles from "./AuthenticationPage.module.css";
import { TEXTS } from "../constants/texts";

interface RegisterPageProps {
  onBackToLogin: () => void;
}

type Step = 1 | 2 | 3;

export default function RegisterPage({ onBackToLogin }: RegisterPageProps) {
  const [step, setStep] = useState<Step>(1);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [code, setCode] = useState("");
  const [username, setUsername] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const passwordsMismatch = confirmPassword !== "" && password !== confirmPassword;

  const run = async (action: () => Promise<void>, next: () => void) => {
    setError("");
    setIsLoading(true);
    try {
      await action();
      next();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Something went wrong");
    } finally {
      setIsLoading(false);
    }
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (step === 1) {
      if (password !== confirmPassword) {
        setError(TEXTS.AUTH.PASSWORDS_MISMATCH);
        return;
      }
      run(() => startRegistration(email, password), () => setStep(2));
    } else if (step === 2) {
      run(() => verifyCode(email, code), () => setStep(3));
    } else {
      run(() => completeRegistration(email, username), onBackToLogin);
    }
  };

  const titles = TEXTS.AUTH.REGISTER_TITLES;

  const buttonLabels: Record<Step, string> = {
    1: isLoading ? "Sending code..." : "Continue",
    2: isLoading ? "Verifying..." : "Verify",
    3: isLoading ? "Creating account..." : "Finish",
  };

  return (
    <div className={styles.authContainer}>
      <p className={styles.switchText}>Step {step} of 3</p>
      <h2 className={styles.title}>{titles[step]}</h2>

      {error && <p className={styles.errorMessage}>{error}</p>}

      <form onSubmit={handleSubmit} className={styles.form}>
        {step === 1 && (
          <>
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
                minLength={8}
                disabled={isLoading}
                autoComplete="new-password"
                className={styles.input}
              />
            </div>

            <div className={styles.fieldGroup}>
              <label>Repeat password</label>
              <input
                type="password"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                required
                disabled={isLoading}
                autoComplete="new-password"
                className={styles.input}
              />
              {passwordsMismatch && (
                <small className={styles.fieldError}>Passwords do not match.</small>
              )}
              <small className={styles.switchText}>{TEXTS.AUTH.PASSWORD_HINT}</small>
            </div>
          </>
        )}

        {step === 2 && (
          <div className={styles.fieldGroup}>
            <label>Verification code</label>
            <small className={styles.switchText}>
              We sent a 6-digit code to {email}.
            </small>
            <input
              type="text"
              inputMode="numeric"
              maxLength={6}
              value={code}
              onChange={(e) => setCode(e.target.value.replace(/\D/g, ""))}
              required
              disabled={isLoading}
              autoComplete="one-time-code"
              className={styles.input}
            />
          </div>
        )}

        {step === 3 && (
          <div className={styles.fieldGroup}>
            <label>Username</label>
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
              minLength={2}
              disabled={isLoading}
              autoComplete="username"
              className={styles.input}
            />
            <small className={styles.switchText}>
              A 4-digit tag (like #0001) is added automatically.
            </small>
          </div>
        )}

        <button
          type="submit"
          disabled={isLoading || (step === 1 && passwordsMismatch)}
          className={styles.submitButton}
        >
          {buttonLabels[step]}
        </button>

        {step === 2 && (
          <button
            type="button"
            className={styles.linkButton}
            disabled={isLoading}
            onClick={() => {
              setError("");
              setCode("");
              setStep(1);
            }}
          >
            Back
          </button>
        )}
      </form>

      <p className={styles.switchText}>
        Already have an account?{" "}
        <button type="button" className={styles.linkButton} onClick={onBackToLogin}>
          Log in
        </button>
      </p>
    </div>
  );
}