import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { joinServer } from "../../services/serverService";

interface InvitePageProps {
  token: string;
}

export default function InvitePage({ token }: InvitePageProps) {
  const { code } = useParams<{ code: string }>();
  const navigate = useNavigate();

  const [joining, setJoining] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleJoin = async () => {
    if (!code) return;

    setJoining(true);
    setError(null);

    try {
      await joinServer(token, code);
      navigate("/");
    } catch (error) {
      setError(
        error instanceof Error
          ? error.message
          : "Failed to join server."
      );
    } finally {
      setJoining(false);
    }
  };

  return (
    <div className="invitePage">
      <h2>Server Invitation</h2>

      <p>You've been invited to join a server.</p>

      {error && <p>{error}</p>}

      <button
        type="button"
        onClick={handleJoin}
        disabled={!code || joining}
      >
        {joining ? "Joining..." : "Join Server"}
      </button>
    </div>
  );
}
