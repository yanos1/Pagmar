Shader "Custom/StencilMask"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ColorMask 0          // Do not write to color buffer (invisible)
        ZWrite Off           // Disable depth write

        Stencil
        {
            Ref 1            // Reference value = 1
            Comp always      // Always pass stencil test
            Pass replace     // Replace stencil buffer with Ref value
        }

        Pass {}
    }
}
