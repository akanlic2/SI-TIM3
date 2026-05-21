import { useState, useEffect, useRef, useCallback } from 'react';
import { fetchQuestions, createQuestion } from '../api/questionApi';
import type { Question } from '../types';

interface QAPanelProps {
  sessionId: string;
  sessionStartTime: string;
  role: string;
}

const POLL_INTERVAL_MS = 10_000;

export default function QAPanel({ sessionId, sessionStartTime, role }: QAPanelProps) {
  const canAsk = role === 'ucesnik' || role === 'predavac';
  const [questions, setQuestions] = useState<Question[]>([]);
  const [content, setContent] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const pollRef = useRef<ReturnType<typeof setInterval> | null>(null);

  const sessionHasStarted = new Date(sessionStartTime) <= new Date();

  const loadQuestions = useCallback(async () => {
    const data = await fetchQuestions(sessionId);
    setQuestions(data);
    setIsLoading(false);
  }, [sessionId]);

  useEffect(() => {
    if (!sessionHasStarted) return;
    loadQuestions();
    pollRef.current = setInterval(loadQuestions, POLL_INTERVAL_MS);
    return () => {
      if (pollRef.current) clearInterval(pollRef.current);
    };
  }, [sessionId, sessionHasStarted, loadQuestions]);

  const handleSubmit = async () => {
    if (!content.trim()) return;
    setIsSubmitting(true);
    setErrorMessage('');
    setSuccessMessage('');
    try {
      await createQuestion(sessionId, { content: content.trim() });
      setContent('');
      setSuccessMessage('Pitanje je uspješno poslano.');
      await loadQuestions();
    } catch (error: unknown) {
      if (typeof error === 'object' && error !== null && 'response' in error) {
        const axiosError = error as { response?: { data?: { error?: string }; status?: number } };
        if (axiosError.response?.status === 403) {
          setErrorMessage('Sesija još nije počela. Pitanja možete postavljati tek nakon početka.');
        } else {
          setErrorMessage(axiosError.response?.data?.error ?? 'Greška pri slanju pitanja.');
        }
      } else {
        setErrorMessage('Greška pri slanju pitanja.');
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!sessionHasStarted) {
    return (
      <div style={{
        background: 'var(--bg-card)',
        border: '1px solid var(--border)',
        borderRadius: 'var(--radius-md)',
        padding: '24px',
        textAlign: 'center',
        color: 'var(--text-muted)',
        fontSize: '0.875rem',
      }}>
        Q&A panel je dostupan tek nakon početka sesije.
      </div>
    );
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>

      {/* Lista pitanja */}
      <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
        {isLoading ? (
          <p style={{ color: 'var(--text-muted)', fontSize: '0.875rem' }}>Učitavanje pitanja...</p>
        ) : questions.length === 0 ? (
          <div className="empty-state">
            <div className="empty-icon">💬</div>
            <p style={{ color: 'var(--text-secondary)', fontSize: '0.875rem' }}>
              Još nema pitanja. Budite prvi!
            </p>
          </div>
        ) : (
          questions.map((q) => (
            <div key={q.questionId} style={{
              background: 'var(--bg-card)',
              border: '1px solid var(--border)',
              borderRadius: 'var(--radius-md)',
              padding: '16px',
            }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: '8px', marginBottom: '8px' }}>
                <span style={{ fontSize: '0.8rem', fontWeight: 600, color: 'var(--text-primary)' }}>
                  {q.authorName}
                </span>
                <span style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>
                  {new Date(q.askedAt).toLocaleTimeString('bs-BA', { hour: '2-digit', minute: '2-digit' })}
                </span>
              </div>

              <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', lineHeight: 1.5, margin: 0 }}>
                {q.content}
              </p>

              {q.answer && (
                <div style={{
                  marginTop: '12px',
                  paddingLeft: '12px',
                  borderLeft: '2px solid var(--accent-blue)',
                  background: 'rgba(63, 131, 248, 0.06)',
                  borderRadius: '0 var(--radius-sm) var(--radius-sm) 0',
                  padding: '10px 12px',
                }}>
                  <p style={{ fontSize: '0.75rem', fontWeight: 600, color: 'var(--accent-blue)', marginBottom: '4px' }}>
                    Odgovor predavača
                  </p>
                  <p style={{ fontSize: '0.875rem', color: 'var(--text-primary)', margin: 0 }}>
                    {q.answer}
                  </p>
                </div>
              )}
            </div>
          ))
        )}
      </div>

      {/* Forma za pitanje */}
      {canAsk && (
      <div style={{
        background: 'var(--bg-card)',
        border: '1px solid var(--border)',
        borderRadius: 'var(--radius-md)',
        padding: '16px',
      }}>
        <textarea
          value={content}
          onChange={(e) => setContent(e.target.value)}
          placeholder="Unesite vaše pitanje..."
          maxLength={500}
          rows={3}
          className="form-textarea"
          style={{ width: '100%', boxSizing: 'border-box' }}
        />
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginTop: '8px' }}>
          <span style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>{content.length}/500</span>
          <button
            onClick={handleSubmit}
            disabled={isSubmitting || !content.trim()}
            className="btn-primary-sm"
          >
            {isSubmitting ? 'Slanje...' : 'Pošalji pitanje'}
          </button>
        </div>

        {successMessage && (
          <p style={{ marginTop: '8px', fontSize: '0.75rem', color: 'var(--accent-green)' }}>
            {successMessage}
          </p>
        )}
        {errorMessage && (
          <p style={{ marginTop: '8px', fontSize: '0.75rem', color: 'var(--accent-red)' }}>
            {errorMessage}
          </p>
        )}
      </div>
      )}
      </div>
  );
}