import React from 'react'
import PropTypes from 'prop-types'
import { useNavigate } from 'react-router-dom';
import { yupResolver } from '@hookform/resolvers/yup';
import { useForm } from 'react-hook-form';
import { categorySchema } from 'validateSchema/CategorySchema';
import { useRef } from 'react';

CategoryForm.propTypes = {}

function CategoryForm(props) {
    const { initialValues, isAddMode } = props;
    let navigate = useNavigate();

    const formSubmit = useRef();
    const btnSubmit = useRef();

    const { register, handleSubmit, formState: { errors }, formState } = useForm({
        resolver: yupResolver(categorySchema),
    });

    const { isSubmitting } = formState;

    function HandleRedirect(redirectUrl) {
        navigate(redirectUrl);
    }

    const HandleTypingFormInput = () => {
        if (
            formSubmit.current.name.value.length > 0 
        ) {
            btnSubmit.current.removeAttribute("disabled");
        }
        else {
            btnSubmit.current.setAttribute("disabled", "disabled");
        }
    };

    return (
        <form 
            onSubmit={handleSubmit(props.onSubmit)}
            ref={formSubmit}
            onInput={HandleTypingFormInput}
        >
            <div className="row">
                <div className="form-group col-md-12">
                    <label htmlFor='name' className='mb-2'>Category Name <span className="text-danger">*</span> <small className="fw-lighter fst-italic">(max: 1000 text)</small></label>
                    <input type="text" className="form-control" {...register("name")} defaultValue={initialValues.name} />
                    <small className='text-danger'>{errors.name?.message}</small>
                </div>
            </div>

            <div className='text-end mt-3'>
                <button 
                    type='submit' 
                    className={`btn btn-primary btn-custom-loading me-2 ${isSubmitting ? "is-loading" : ""}`}
                    ref={btnSubmit}
                    disabled
                >
                    <div className="loader"></div>
                    <span>Submit</span>
                </button>
            </div>
        </form>
    )
}



export default CategoryForm
