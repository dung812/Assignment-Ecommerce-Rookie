import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import productApi from "api/productAPI";
import { toast } from "react-toastify";

const product = createSlice({
    name: "products",
    initialState: { loading: false, products: [], description: '' },
    reducers: {
        searchProduct: (state, action) => {
            state.products = action.payload
        },
        setDescription: (state, action) => {
            state.description = action.payload
        }
    },
    extraReducers: builder => {
        builder
            .addCase(fetchProducts.pending, (state, action) => {
                state.loading = true;
            })
            .addCase(fetchProducts.fulfilled, (state, action) => {
                state.loading = false;
                state.products = action.payload;
            })
            .addCase(fetchProducts.rejected, (state, action) => {
                state.loading = false;
                state.products = [];
            })

            .addCase(addNewProduct.pending, (state, action) => {
                state.loading = true;
            })
            .addCase(addNewProduct.fulfilled, (state, action) => {
                state.loading = false;
                state.products.push(action.payload)

                toast.success('Successfully add new product', {
                    position: "top-right",
                    autoClose: 5000,
                    hideProgressBar: false,
                    closeOnClick: true,
                    pauseOnHover: true,
                    draggable: true,
                    progress: undefined,
                    theme: "light",
                });
            })

            .addCase(updateProduct.pending, (state, action) => {
                state.loading = true;
            })
            .addCase(updateProduct.fulfilled, (state, action) => {
                state.loading = false;
                state.products = action.payload;

                toast.success('Successfully edit product', {
                    position: "top-right",
                    autoClose: 5000,
                    hideProgressBar: false,
                    closeOnClick: true,
                    pauseOnHover: true,
                    draggable: true,
                    progress: undefined,
                    theme: "light",
                });
            })


            .addCase(deleteProduct.pending, (state, action) => {
                state.loading = true;
            })
            .addCase(deleteProduct.fulfilled, (state, action) => {
                state.loading = false;
                state.products = action.payload;
            })
    }
})

const { reducer, actions } = product;
export const { searchProduct, setDescription } = actions;
export default reducer;

// 
/*
    M???i m???t createAsyncThunk s??? c?? 3 action:
    * pending: khi v???a g???i request
    * fullfilled: tr???ng th??i th??nh c??ng
    * rejected: tr???ng th??i th???t b???i
*/
export const fetchProducts = createAsyncThunk('products/fetchProducts', async () => {
    const res = await productApi.getAll();
    return res
})
export const fetchProduct = createAsyncThunk('products/fetchProduct', async (productId) => {
    const res = await productApi.getProduct(productId);
    return res
})

export const addNewProduct = createAsyncThunk('products/addNewProduct', async (newProduct) => {
    await productApi.addProduct(newProduct);

    // Sau khi add xong, g???i l???i product list m???i nh???t v??? v?? update v??o state product
    const products = await productApi.getAll();
    return products
})

export const updateProduct = createAsyncThunk('products/updateProduct', async (data) => {
    await productApi.updateProduct(parseInt(data.ProductId), data);

    // Sau khi update xong, g???i l???i product list m???i nh???t v??? v?? update v??o state product
    const products = await productApi.getAll();
    return products
})

export const deleteProduct = createAsyncThunk('products/deleteProduct', async (productId) => {
    await productApi.deleteProduct(productId);

    // Sau khi x??a xong, g???i l???i product list m???i nh???t v??? v?? update v??o state product
    const products = await productApi.getAll();
    return products
})

// Dispatch v??o thunk action creators tr??? ra thunk action
/*
    Thay v?? dispatch v??o action th??ng th?????ng, n???u c???n x??? l?? side effect th?? ta s??? dispatch v??o thunk action creators
    trong thunk action s??? x??? l?? b???t ?????ng b??? v?? s??? dispatch v??o action ch??nh
        - getState: tr??? ra t???t c??? c??c state hi???n t???i ??? state chung
        - dispatch: ????? dispatch t???i action ch??nh
*/

// export function addProducts(product) {
//     return function addProductThunk(dispatch, getState) {
//         console.log('[add product thunk]', getState())
//         console.log({product})
//         product.ProductName = "t??t"
//         const action = addProduct(product)
//         dispatch(action)
//         console.log('[add product thunk]', getState())
//     }
// }