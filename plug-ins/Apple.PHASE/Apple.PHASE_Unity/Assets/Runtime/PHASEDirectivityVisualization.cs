using UnityEngine;

namespace Apple.PHASE
{
    /// <summary>
    /// Class used to handle directivity visualization of <c>PHASESpatialMixer</c> in the UnityEditor.
    /// </summary>
    /// <see cref="PHASESpatialMixer"/>
    public static class PHASEDirectivityVisualization
    {
        private static float _radius = 5f;

        /// <summary>
        /// Generates a cardioid based on the given parameters.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="sharpness"></param>
        /// <returns> A <c>Mesh</c> based on the given parameters.</returns>
        public static Mesh GenerateCardioidMesh(float pattern, float sharpness)
        {
            return GenerateCardioidMeshFromVertices(GenerateCardioidByAzimuth(pattern, sharpness, Mathf.PI * 0.5f, 150, _radius));
        }

        /// <summary>
        /// Generate an arc mesh based on the given parameters.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        /// <remarks>Angle is in radians.</remarks>
        public static Mesh GenerateArcMesh(float angle)
        {
            int steps = 150;
            float step = angle / steps;
            Vector3[] vertices = new Vector3[steps + 1];
            int[] triangles = new int[steps * 6];

            float startAngle = (Mathf.PI - angle) / 2f;
            vertices[0] = Vector3.zero; // center point of circle
            for (int i = 1; i <= steps; i++)
            {
                int index = (i - 1) * 6;
                float curAngle = (i - 1) * step;
                float curX = _radius * Mathf.Cos(startAngle + curAngle);
                float curY = _radius * Mathf.Sin(startAngle + curAngle);
                vertices[i] = new Vector3(curY, 0f, curX);

                if (angle == Mathf.PI * 2f) // if angle is 2PI then connect circle edges
                {
                    triangles[index] = 1;
                }
                else
                {
                    triangles[index] = 0;
                }

                triangles[index + 1] = i;

                // wraparound the last vertex
                if (i == steps)
                {
                    triangles[index + 2] = 0;
                }
                else
                {
                    triangles[index + 2] = i + 1;
                };

                // Additional triangles to render backface of mesh
                triangles[index + 3] = triangles[index];
                triangles[index + 4] = triangles[index + 2];
                triangles[index + 5] = triangles[index + 1];
            }
            Mesh arc = new Mesh
            {
                vertices = vertices,
                triangles = triangles
            };
            arc.RecalculateBounds();
            arc.RecalculateNormals();
            arc.RecalculateTangents();

            return arc;
        }


        // Generate a cardioid mesh based on set of points.
        private static Mesh GenerateCardioidMeshFromVertices(Vector3[] vertices)
        {
            int[] triangles = new int[vertices.Length * 6];
            vertices[0] = Vector3.zero; // center point of cardioid
            for (int i = 0; i < vertices.Length - 1; i++)
            {
                int index = i * 6;

                if (i < vertices.Length - 2)
                {
                    triangles[index] = 0;
                    triangles[index + 1] = i + 1;
                    triangles[index + 2] = i + 2;
                }
                // wraparound last triangle
                else
                {
                    triangles[index] = 0;
                    triangles[index + 1] = vertices.Length - 1;
                    triangles[index + 2] = 1;
                }

                // Additional triangles to render backface of mesh
                triangles[index + 3] = triangles[index];
                triangles[index + 4] = triangles[index + 2];
                triangles[index + 5] = triangles[index + 1];
            }
            Mesh cardioid = new Mesh
            {
                vertices = vertices,
                triangles = triangles
            };
            cardioid.RecalculateBounds();
            cardioid.RecalculateNormals();
            cardioid.RecalculateTangents();

            return cardioid;
        }

        private static Vector3[] GenerateCardioidByAzimuth(float pattern, float sharpness, float inclination, int steps, float scale)
        {
            float stepSize = Mathf.PI * 2 / steps;
            Vector3[] vertices = new Vector3[steps];

            for (int i = 0; i < steps; i++)
            {
                float radius = CalculateCardioidGain(inclination, i * stepSize, pattern, sharpness) * scale;
                vertices[i] = SphericalToCartesian(radius, inclination, i * stepSize);
            }

            return vertices;
        }

        private static float CalculateCardioidGain(float inclination, float azimuth, float pattern, float sharpness)
        {
            float angle = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            // Adapted from PHASE:Interpolation::Linear.
            float radius = Mathf.Abs(1 - pattern + (pattern * angle));
            return Mathf.Pow(radius, sharpness);
        }

        private static Vector3 SphericalToCartesian(float radius, float inclination, float azimuth)
        {
            float si = Mathf.Sin(inclination);
            float x = radius * si * Mathf.Sin(azimuth);
            float y = radius * si * Mathf.Cos(azimuth);
            float z = radius * Mathf.Cos(inclination);

            // Y is up in Unity, so swap z and y.
            return new Vector3(x, z, y);
        }
    }
}
