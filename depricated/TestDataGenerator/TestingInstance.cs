using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDataGenerator
{
    public record struct TestingInstance(TestData Properties, byte[] Unsorted, byte[] Sorted);
}
