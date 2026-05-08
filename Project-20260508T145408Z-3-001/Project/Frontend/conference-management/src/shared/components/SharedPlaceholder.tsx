import type { PropsWithChildren } from 'react'

interface BaseCardProps extends PropsWithChildren {
  title: string
}

export function BaseCard({ title, children }: BaseCardProps) {
  return (
    <section style={{ border: '1px solid #ddd', borderRadius: '8px', padding: '12px' }}>
      <h3>{title}</h3>
      <div>{children}</div>
    </section>
  )
}
