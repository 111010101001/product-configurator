import { useState, useEffect } from 'react';

type ProductSummary = { id: number; name: string; sku: string; basePrice: number }
type Status = 'loading' | 'loaded' | 'error'

function App() {
  const [products, setProducts] = useState<ProductSummary[]>([])
  const [status, setStatus] = useState<Status>('loading')

  useEffect(() => {
    const controller = new AbortController()
    void (async () => {
      try{
          const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/api/products`, { signal: controller.signal });
          
          if (!response.ok) {
            throw new Error(`Response status: ${response.status}`);
          }
          setProducts(await response.json())
          setStatus('loaded')
      }
      catch{
        if (!controller.signal.aborted) setStatus('error')
      }
    })()

    return () => controller.abort()
  }, [])
  
  return status !== 'loaded' ? <p>Status: {status}</p> : <ul>{products.map(prod => <li key={prod.id}>{prod.id},{prod.name},{prod.basePrice}</li>)}</ul>
  // render "Loading…", the error, or a <ul> with one <li key={p.id}> per product
}

export default App