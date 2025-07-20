using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class MachineFlagsPlugin
{
	// Mock data for the lights
	private readonly List<MachineModel> machines = new()
   {
	  new MachineModel { Id = 1, Name = "Service Elevator", RepairNeeded = false, InspectionDueDate = DateTime.Today },
	  new MachineModel { Id = 2, Name = "Lift A", RepairNeeded = false, InspectionDueDate = DateTime.Today },
	  new MachineModel { Id = 3, Name = "Lift B", RepairNeeded = true, InspectionDueDate = DateTime.Today },
	  new MachineModel { Id = 4, Name = "Torch", RepairNeeded = true, InspectionDueDate = DateTime.Today }
   };

	[KernelFunction("get_machines")]
	[Description("Gets a list of machines and their current state")]
	public async Task<List<MachineModel>> GetMachinesAsync()
	{
		return machines;
	}

	[KernelFunction("update_inspection")]
	[Description("Changes the inspection due date.")]
	public async Task<MachineModel?> UpdateInspectionDateAsync(int id)
	{
		var machine = machines.FirstOrDefault(light => light.Id == id);

		if (machine == null)
		{
			return null;
		}

		// Update the light with the new state
		machine.InspectionDueDate = DateTime.Today.AddMonths(3);

		return machine;
	}
	public class MachineModel
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("repair_needed")]
		public bool? RepairNeeded { get; set; }
		[JsonPropertyName("inspection_due_date")]
		public DateTime? InspectionDueDate { get; set; }

	}
}
