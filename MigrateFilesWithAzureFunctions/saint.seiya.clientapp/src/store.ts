import { applyMiddleware, createStore, Store } from 'redux';
import reduxThunk from 'redux-thunk';
import { httpMiddleware } from './middlewares';

import { state, StateReducer } from './reducers';
import { createLogger } from 'redux-logger';

let middlewares = [reduxThunk, httpMiddleware];

if (process.env.REACT_APP_BUILD === 'LOCAL' || process.env.REACT_APP_BUILD === 'INT') {
	middlewares = [...middlewares, createLogger()];
}

export const store: Store<StateReducer> = createStore(state, applyMiddleware(...middlewares));
