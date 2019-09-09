Shader "Custom/zbuffer" {
		Properties{
			_Color("Main Color", Color) = (1,1,1,1)
			_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		}
			SubShader{
			Tags{ "Queue" = "Geometry+2499" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			LOD 200
			// extra pass that renders to depth buffer only
			Pass{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			ColorMask 0
		}
			// paste in forward rendering passes from Transparent/Diffuse
			UsePass "Transparent/Diffuse/FORWARD"
		}
			Fallback "VertexLit"
	}