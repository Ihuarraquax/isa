using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NumberFormatManager.Services;

namespace PSOAlgorithmModule
{
    public class PSOAlgorithm
    {
        public int A { get; }
        public int B { get; }
        public decimal D { get; }
        public int N { get; }
        public int T { get; }
        public decimal C1 { get; }
        public decimal C2 { get; }
        public decimal C3 { get; }
        public decimal Rs { get; }

        public readonly Random random;
        public NumberFormatService Manager;

        public List<Particle> Particles { get; set; }

        public ValueModel Result => Particles.SelectMany(_ => _.Values).MaxBy(_ => _.Fx).FirstOrDefault();
        public List<Slajd> Slajds { get; set; }

        public PSOAlgorithm(int a, int b, decimal d, int n, int t, decimal c1, decimal c2, decimal c3, decimal rs)
        {
            A = a;
            B = b;
            D = d;
            N = n;
            T = t;
            C1 = c1;
            C2 = c2;
            C3 = c3;
            Rs = rs;

            Particles = new List<Particle>();
            random = new Random();
            Manager = new NumberFormatService(a, b, d, random);
        }

        public void Run()
        {
            CreateSwarm();
            for (int i = 0; i < T; i++)
            {
                if (IsDone()) break;
                UpdateOwnKnowledge();
                UpdateGlobalKnowledge();
                MoveParticles();
            }
        }


        private void MoveParticles()
        {
            for (int i = 0; i < N; i++)
            {
                var particle = Particles[i];
                var x = particle.Values.Last().X;


                var r1 = (decimal) random.NextDouble();
                var c1 = (C1 * r1 * particle.V);

                var r2 = (decimal) random.NextDouble();
                var c2 = (C2 * r2 * (particle.Bi.X - x));

                var r3 = (decimal) random.NextDouble();
                var c3 = (C3 * r3 * (particle.Bg.X - x));

                var v = c1 + c2 + c3;

                if (x + v < A)
                {
                    particle.Values.Add(new ValueModel(A, Manager.CalculateFx(A)));
                    continue;
                }

                if (x + v > B)
                {
                    particle.Values.Add(new ValueModel(B, Manager.CalculateFx(B)));
                    continue;
                }

                particle.V = v;
                particle.Values.Add(new ValueModel(x + v, Manager.CalculateFx(x + v)));
            }
        }


        private void UpdateOwnKnowledge()
        {
            for (int i = 0; i < N; i++)
            {
                var particle = Particles[i];
                if (particle.Values.Last().Fx > particle.Bi.Fx)
                {
                    particle.Bi = particle.Values.Last();
                }
            }
        }

        private void UpdateGlobalKnowledge()
        {
            for (int i = 0; i < N; i++)
            {
                var particle = Particles[i];
                var particleX = particle.Values.Last().X;
                var bestInGlobal = Particles
                    .Where(_ => Math.Abs(_.Values.Last().X - particleX) <= Rs)
                    .MaxBy(_ => _.Values.Last().Fx)
                    .Select(_ => _.Values.Last())
                    .FirstOrDefault();
                // var bestInGlobal = Particles.MaxBy(_ => _.Values.Last().Fx).Select(_ => _.Values.Last()).FirstOrDefault();
                
                if (bestInGlobal != null && bestInGlobal.Fx > particle.Bg.Fx)
                {
                    particle.Bg = bestInGlobal;
                }
                
            }
        }

        private void CreateSwarm()
        {
            var r = (decimal) random.NextDouble();
            for (int i = 0; i < N; i++)
            {
                var firstValue = Manager.RandomDecimal();
                var valueModel = new ValueModel(firstValue, Manager.CalculateFx(firstValue));
                Particles.Add(new Particle
                {
                    Values = new List<ValueModel>()
                    {
                        valueModel
                    },
                    Bi = valueModel,
                    Bg = valueModel
                });
            }
        }

        private bool IsDone()
        {
            var minX = Particles.Min(_ => _.Values.Last().X);
            var maxX = Particles.Max(_ => _.Values.Last().X);
            if (maxX - minX <= D)
            {
                return true;
            }

            return false;
        }
    }

    public class Particle
    {
        public List<ValueModel> Values { get; set; }

        // Fał
        public decimal V { get; set; } = 0;

        //Najlepsze znalezione rezwiazanie
        public ValueModel Bi { get; set; }

        //Najlepsze rozwiązanie globalne
        public ValueModel Bg { get; set; }
    }

    public class ValueModel
    {
        public decimal X { get; }
        public decimal Fx { get; }

        public ValueModel(decimal x, decimal fx)
        {
            X = x;
            Fx = fx;
        }
    }

    public class Slajd
    {
        public double[] valuesX;
        public double[] valuesY;
    }
}