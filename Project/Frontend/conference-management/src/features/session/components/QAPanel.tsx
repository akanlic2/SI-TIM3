import { useState, useEffect, useRef, useCallback } from 'react';
import { fetchQuestions, createQuestion, answerQuestion } from '../api/questionApi';
import type { Question } from '../types';

interface QAPanelProps {
  sessionId: string;
  sessionStartTime: string;
  sessionEndTime: string;
  role: string;
  canAnswer: boolean;
  canAsk: boolean;
}

const POLL_INTERVAL_MS = 10_000;
const COOLDOWN_SECONDS = 30;

export default function QAPanel({ sessionId, sessionStartTime, sessionEndTime, canAnswer, canAsk }: QAPanelProps) {
  const [questions, setQuestions] = useState<Question[]>([]);
  const [content, setContent] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [cooldownRemaining, setCooldownRemaining] = useState(0);
  const [activeAnswerId, setActiveAnswerId] = useState<string | null>(null);
  const [answerDraft, setAnswerDraft] = useState('');
  const [answeredOrally, setAnsweredOrally] = useState(false);
  const [isSavingAnswer, setIsSavingAnswer] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const pollRef = useRef<ReturnType<typeof setInterval> | null>(null);

  const sessionHasStarted = new Date(sessionStartTime) <= new Date();
  const sessionHasEnded = new Date(sessionEndTime) < new Date();
  const canAskNow = canAsk && !sessionHasEnded;

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

  useEffect(() => {
    if (cooldownRemaining <= 0) return;

    const timer = setInterval(() => {
      setCooldownRemaining((prev) => Math.max(0, prev - 1));
    }, 1000);

    return () => clearInterval(timer);
  }, [cooldownRemaining]);

  const handleSubmit = async () => {
    if (!content.trim() || cooldownRemaining > 0) return;
    setIsSubmitting(true);
    setErrorMessage('');
    setSuccessMessage('');
    try {
      await createQuestion(sessionId, { content: content.trim() });
      setContent('');
      setSuccessMessage('Pitanje je uspješno poslano.');
      setCooldownRemaining(COOLDOWN_SECONDS);
      await loadQuestions();
    } catch (error: unknown) {
      if (typeof error === 'object' && error !== null && 'response' in error) {
        const axiosError = error as { response?: { data?: { error?: string; retryAfterSeconds?: number }; status?: number } };
        if (axiosError.response?.status === 403) {
          setErrorMessage('Sesija još nije počela. Pitanja možete postavljati tek nakon početka.');
        } else if (axiosError.response?.status === 429) {
          const retryAfter = axiosError.response?.data?.retryAfterSeconds;
          if (typeof retryAfter === 'number' && retryAfter > 0) {
            setCooldownRemaining(retryAfter);
          }
          setErrorMessage(axiosError.response?.data?.error ?? 'Previše pitanja. Pokušajte ponovo kasnije.');
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

  const startAnswer = (question: Question) => {
    setActiveAnswerId(question.questionId);
    setAnswerDraft(question.answer ?? '');
    setAnsweredOrally(false);
  };

  const cancelAnswer = () => {
    setActiveAnswerId(null);
    setAnswerDraft('');
    setAnsweredOrally(false);
  };

  const handleAnswerSave = async (questionId: string) => {
    if (!answerDraft.trim() && !answeredOrally) return;

    setIsSavingAnswer(true);
    setErrorMessage('');
    setSuccessMessage('');

    try {
      await answerQuestion(sessionId, questionId, {
        answer: answerDraft.trim(),
        answeredOrally,
      });
      await loadQuestions();
      cancelAnswer();
    } catch (error: unknown) {
      if (typeof error === 'object' && error !== null && 'response' in error) {
        const axiosError = error as { response?: { data?: { error?: string } } };
        setErrorMessage(axiosError.response?.data?.error ?? 'Greška pri slanju odgovora.');
      } else {
        setErrorMessage('Greška pri slanju odgovora.');
      }
    } finally {
      setIsSavingAnswer(false);
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
    <div className="qa-panel">

      {/* Lista pitanja */}
      <div className="qa-panel-list">
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
          [...questions]
            .sort((a, b) => {
              const isOpenA = a.status?.toLowerCase() === 'open';
              const isOpenB = b.status?.toLowerCase() === 'open';

              if (isOpenA !== isOpenB) return isOpenA ? -1 : 1;

              return new Date(a.askedAt).getTime() - new Date(b.askedAt).getTime();
            })
            .map((q) => {
            const hasAnswer = Boolean(q.answer && q.answer.trim());
            const isActive = activeAnswerId === q.questionId;
            const canSave = Boolean(answerDraft.trim() || answeredOrally);
            const isOpen = q.status?.toLowerCase() === 'open';

            return (
              <div key={q.questionId} className="qa-question-card" style={{
                background: 'var(--bg-card)',
                border: '1px solid var(--border)',
                borderRadius: 'var(--radius-md)',
                padding: '16px',
              }}>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', gap: '8px', marginBottom: '8px' }}>
                  <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                    <span style={{ fontSize: '0.8rem', fontWeight: 600, color: 'var(--text-primary)' }}>
                      {q.authorName}
                    </span>
                    <span style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>
                      {new Date(q.askedAt).toLocaleTimeString('bs-BA', { hour: '2-digit', minute: '2-digit' })}
                    </span>
                    {isOpen && (
                      <span
                        aria-label="Otvoreno"
                        style={{
                          width: '8px',
                          height: '8px',
                          borderRadius: '9999px',
                          background: 'var(--accent-amber)',
                          boxShadow: '0 0 6px rgba(245, 158, 11, 0.6)',
                          display: 'inline-block',
                        }}
                      />
                    )}
                  </div>

                  {canAnswer && (
                    <button
                      className="btn-secondary btn-qa-answer"
                      onClick={() => (isActive ? cancelAnswer() : startAnswer(q))}
                    >
                      Odgovori
                    </button>
                  )}
                </div>

                <p style={{ fontSize: '0.875rem', color: 'var(--text-secondary)', lineHeight: 1.5, margin: 0 }}>
                  {q.content}
                </p>

                {hasAnswer && (
                  <div style={{
                    marginTop: '12px',
                    marginLeft: '12px',
                    paddingLeft: '12px',
                    borderLeft: '2px solid var(--accent-blue)',
                    background: 'rgba(59, 130, 246, 0.08)',
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

                {canAnswer && isActive && (
                  <div style={{
                    marginTop: '12px',
                    display: 'flex',
                    flexDirection: 'column',
                    gap: '10px',
                  }}>
                    <textarea
                      value={answerDraft}
                      onChange={(event) => setAnswerDraft(event.target.value)}
                      placeholder="Unesite odgovor..."
                      rows={3}
                      className="form-textarea"
                      style={{ width: '100%', boxSizing: 'border-box' }}
                    />

                    <label style={{ display: 'flex', alignItems: 'center', gap: '8px', fontSize: '0.875rem', color: 'var(--text-secondary)' }}>
                      <input
                        type="checkbox"
                        checked={answeredOrally}
                        onChange={(event) => setAnsweredOrally(event.target.checked)}
                      />
                      Odgovoreno usmeno
                    </label>

                    <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '8px' }}>
                      <button
                        className="btn-secondary"
                        onClick={cancelAnswer}
                      >
                        Otkaži
                      </button>
                      <button
                        className="btn-primary-sm"
                        onClick={() => handleAnswerSave(q.questionId)}
                        disabled={!canSave || isSavingAnswer}
                      >
                        {isSavingAnswer ? 'Spremanje...' : 'Spasi'}
                      </button>
                    </div>
                  </div>
                )}
              </div>
            );
          })
        )}
      </div>

      {/* Forma za pitanje */}
      {canAskNow && (
      <div className="qa-panel-form" style={{
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
            disabled={isSubmitting || !content.trim() || cooldownRemaining > 0}
            className="btn-primary-sm"
          >
            {isSubmitting ? 'Slanje...' : cooldownRemaining > 0 ? `Sačekajte ${cooldownRemaining}s` : 'Pošalji pitanje'}
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