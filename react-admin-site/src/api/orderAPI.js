import axiosClient from "./axiosClient";

const orderApi = {
  getAll: (status) => {
    const url = `/Order/${status}`;
    return axiosClient.get(url); 
  },
  checkedOrder: (orderId) => {
    const url = `/Order/CheckedOrder/${orderId}`;
    return axiosClient.get(url); 
  },
  successOrder: (orderId) => {
    const url = `/Order/SuccessDeliveryOrder/${orderId}`;
    return axiosClient.get(url); 
  },
  cancelledOrder: (orderId) => {
    const url = `/Order/CancellationOrder/${orderId}`;
    return axiosClient.get(url); 
  },
  deleteOrder: (orderId) => {
    const url = `/Order/${orderId}`;
    return axiosClient.delete(url); 
  },
  // edit, remove, ...
}
export default orderApi;