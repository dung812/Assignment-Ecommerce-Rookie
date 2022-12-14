import React, { useEffect } from 'react'
import { useForm } from "react-hook-form";
import { yupResolver } from '@hookform/resolvers/yup';
import { useNavigate } from 'react-router-dom';
import * as yup from "yup";
import { useSelector } from 'react-redux';
import { useRef } from 'react';

function ManufactureForm(props) {
    const { initialValues, isAddMode, onSubmit } = props;
    let navigate = useNavigate();

    const formSubmit = useRef();
    const btnSubmit = useRef();

    let loading = useSelector((state) => state.manufactures.loading);

    useEffect(() => {
        const inputImage = document.getElementById("input-image");
        inputImage && inputImage.addEventListener("change", function (e) {
            readURL(inputImage, "#logo-image");
        })
    }, []);

    function readURL(input, imageSeletor) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                const image = document.querySelector(imageSeletor);
                image && image.setAttribute('src', e.target.result);
            }
            reader.readAsDataURL(input.files[0]);
        }
    };

    const manufactureSchema = yup.object().shape({
        name: yup.string().max(1000, 'Must be exactly 1000 digits').required("Name field should be required please")
    })


    const { register, handleSubmit, formState: { errors }, formState } = useForm({
        resolver: yupResolver(manufactureSchema),
    });

    function HandleRedirect(redirectUrl) {
        navigate(redirectUrl);
    }

    const HandleTypingFormInput = () => {
        if(isAddMode) {
            if (
                formSubmit.current.name.value.length > 0 &&
                formSubmit.current.image.value.length > 0
            ) {
                btnSubmit.current.removeAttribute("disabled");
            }
            else {
                btnSubmit.current.setAttribute("disabled", "disabled");
            }
        }
        else {
            if (
                formSubmit.current.name.value.length > 0
            ) {
                btnSubmit.current.removeAttribute("disabled");
            }
            else {
                btnSubmit.current.setAttribute("disabled", "disabled");
            }
        }
    };

    return (
        <form 
            onSubmit={handleSubmit(onSubmit)}
            onInput={HandleTypingFormInput}
            ref={formSubmit}
        >
            <div className="row">
                <div className="form-group col-md-12">
                    <label htmlFor='name'>Name <span className="text-danger">*</span> <small class="fw-lighter fst-italic">(max: 1000 text)</small></label>
                    <input type="text" className="form-control" {...register("name")} defaultValue={initialValues.name} />
                    <small className='text-danger'>{errors.name?.message}</small>
                </div>

                <div className="form-group col-md-12">
                    <label htmlFor=''>Logo <span className="text-danger">*</span></label>
                    <img src={!isAddMode ? `http://ntdung812-001-site1.btempurl.com/images/brand/${initialValues.logo}` : ""} width="30%" className='img-fluid d-block mx-auto mb-2' id='logo-image' />
                    <input type="file" className="form-control" id='input-image' accept="image/*" name="image" />
                </div>
            </div>

            <div className='text-start mt-3'> 
                <button 
                    type='submit' 
                    className={`btn btn-primary btn-custom-loading me-2 ${loading ? "is-loading" : ""}`}
                    ref={btnSubmit}
                    disabled={isAddMode}
                >
                    <div className="loader"></div>
                    <span>Submit</span>
                </button>
                <button type='button' className='btn btn-outline-dark' onClick={() => HandleRedirect("/manufacture")} >Cancel</button>
            </div>
        </form>
    )
}


export default ManufactureForm
