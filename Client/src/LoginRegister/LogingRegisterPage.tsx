import { useState, type FormEvent } from 'react'
import type { AuthMode } from '../PageTypes'

type LoginRegisterPageProps = {
    onLogin: (username: string) => void
}

function LoginRegisterPage({ onLogin }: LoginRegisterPageProps) {
    const [mode, setMode] = useState<AuthMode>('login')

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

            setMessage('Registration successful')

            // After registering, switch to login
            setMode('login')
            setPassword('')
        } catch (error) {
            console.error(error)
            setMessage('Could not connect to server')
        }
    }

    async function login() {
        try {
            const response = await fetch(
                'http://localhost:5038/api/users/login',
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
                if (response.status === 401) {
                    setMessage('Invalid username or password')
                    return
                }

                setMessage('Login failed')
                return
            }

            // Tell App.tsx login succeeded
            onLogin(username)
        } catch (error) {
            console.error(error)
            setMessage('Could not connect to server')
        }
    }

    function handleSubmit(event: FormEvent<HTMLFormElement>) {
        // Prevent browser from refreshing the page
        event.preventDefault()

        if (username.trim() === '' || password === '') {
            setMessage('Username and password are required')
            return
        }

        if (mode === 'login') {
            login()
        } else {
            register()
        }
    }

    function switchMode() {
        setMode(mode === 'login' ? 'register' : 'login')

        setUsername('')
        setPassword('')
        setMessage('')
    }

    return (
        <div>
            <button
                type="button"
                onClick={switchMode}
            >
                {mode === 'login' ? 'Register' : 'Login'}
            </button>

            <h1>
                {mode === 'login' ? 'Login' : 'Register'}
            </h1>

            <form onSubmit={handleSubmit}>
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

                <button type="submit">
                    {mode === 'login' ? 'Login' : 'Register'}
                </button>
            </form>

            <p>{message}</p>
        </div>
    )
}

export default LoginRegisterPage