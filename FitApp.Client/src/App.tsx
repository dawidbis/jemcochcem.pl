import { useState } from 'react'
import './App.css'

function App() {
  const [status, setStatus] = useState('Idle')

  const testConnection = async () => {
    setStatus('Sending...')
    
    // Mock data matching AddMealItemCommand.cs
    const payload = {
      userId: "00000000-0000-0000-0000-000000000000", // Empty GUID for test
      date: new Date().toISOString(),
      foodProductId: "00000000-0000-0000-0000-000000000000", 
      grams: 150.5
    }

    try {
      const response = await fetch('/diary', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      })

      if (response.ok) {
        setStatus('Success! 200 OK')
      } else {
        setStatus(`Error: ${response.status}`)
      }
    } catch (err:unknown) {
      if (err instanceof Error) {
    setStatus(`Failed: ${err.message}`);
  } else {
    setStatus("Failed: Unknown error");
  }
    }
  }

  return (
    <div style={{ padding: '2rem', textAlign: 'center' }}>
      <h1>FitApp Integration Test</h1>
      <div style={{ margin: '1rem 0', fontWeight: 'bold' }}>
        Status: {status}
      </div>
      <button onClick={testConnection}>
        Send POST to /diary
      </button>
    </div>
  )
}

export default App