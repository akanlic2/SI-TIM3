export type EntityId = string

export interface ApiResult<T> {
	data: T
	message: string
}
