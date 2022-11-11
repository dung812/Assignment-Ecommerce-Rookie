import { configureStore, ThunkAction, Action } from '@reduxjs/toolkit';
import productReducer from '../features/Product/ProductSlice'
import customerReducer from '../features/Customer/CustomerSlice'
import categoryReducer from '../features/Category/CategorySlice'
import manufactureReducer from '../features/Manufacture/ManufactureSlice'
import orderReducer from '../features/Order/OrderSlice'
import authReducer from '../features/Auth/AuthSlice'

const rootReducer = {
	products: productReducer,
    customers: customerReducer,
    categories: categoryReducer,
    manufactures: manufactureReducer,
    orders: orderReducer,
    authAdmin: authReducer,
	// user: userReducer
}

const store = configureStore({
	reducer: rootReducer
});

export default store;