import { useState } from 'react'

function LoginRegisterPage() {
    const [mode, setMode] = useState<'login' | 'register'>('login')

    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const [message, setMessage] = useState('')

    async function register() {
        try {
            const response = await fetch(
                'http://localhost:5038/api/users/add-user',
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        username,
                        passwordHash: password,
                    }),
                }
            )

            if (!response.ok) {
                if (response.status === 409) {
                    setMessage('Username already exists')
                    return
                }

                setMessage('Registration failed')
                return
            }
        } catch (error) {
            console.error(error)
            setMessage('Could not connect to server')
        }
    }

    async function login() {
        // login API logic will go here
    }

    function switchMode() {
        setMode(mode === 'login' ? 'register' : 'login')

        setUsername('')
        setPassword('')
        setMessage('')
    }

    return (
        <div>
            <button onClick={switchMode}>
                {mode === 'login' ? 'Register' : 'Login'}
            </button>

            <h1>
                {mode === 'login' ? 'Login' : 'Register'}
            </h1>

            <input
                type="text"
                placeholder="Username"
                value={username}
                onChange={(event) => setUsername(event.target.value)}
            />

            <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
            />

            {mode === 'login' ? (
                <button onClick={login}>Login</button>
            ) : (
                <button onClick={register}>Register</button>
            )}

            <p>{message}</p>
        </div>
    )
}

export default LoginRegisterPage