import serial
import matplotlib.pyplot as plt
from matplotlib import animation

# Set up the serial connection to the Arduino
class SerialReader:
    def __init__(self, port, baud_rate):
        self.port = port
        self.baud_rate = baud_rate
        self.serial_conn = None

    def open_connection(self):
        try:
            self.serial_conn = serial.Serial(self.port, self.baud_rate, timeout=1)
            print(f"Connected to {self.port} at {self.baud_rate} baud.")
        except serial.SerialException as e:
            print(f"Failed to connect: {e}")

    def read_data(self):
        if self.serial_conn and self.serial_conn.is_open:
            try:
                if self.serial_conn.in_waiting > 0:
                    data = self.serial_conn.readline().decode('utf-8').rstrip()
                    return data
            except serial.SerialException as e:
                print(f"Error reading data: {e}")
        return None

    def close_connection(self):
        if self.serial_conn and self.serial_conn.is_open:
            self.serial_conn.close()
            print("Serial connection closed.")

port = "COM3"  # Replace with your device's port
baud_rate = 115200  # Replace with your device's baud rate

serial_reader = SerialReader(port, baud_rate)
serial_reader.open_connection()


# Create a figure for the plot
fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')
ax.set_xlabel('X')
ax.set_ylabel('Y')
ax.set_zlabel('Z')
ax.set_xlim([-2, 2])
ax.set_ylim([-2, 2])
ax.set_zlim([-2, 2])

# Initialize the line object
line, = ax.plot([0, 0], [0, 0], [0, 0], 'ro-', lw=2)

def parse_data(data_string):
    if data_string:    
        if "A:" in data_string and "G:" in data_string:
            split_data = data_string.split(";")
            
            if len(split_data) >= 2:
                acc_data = split_data[0].split(":")[1].split(",")
                gyro_data = split_data[1].split(":")[1].split(",")
                
                return acc_data, gyro_data
    
    return None, None

# Set up the update function for the plot
def update_plot(frame):
    # Read the accelerometer data from the Arduino\
    data_string = serial_reader.read_data()
    acc_data, gyro_data = parse_data(data_string)
    if acc_data is None or gyro_data is None:
        return
    
    # Extract the accelerometer data
    x, y, z = [float(val) for val in acc_data]
    print(f"Acc: {x}, {y}, {z}")
    # Extract the gyro data
    x_gyro, y_gyro, z_gyro = [float(val) for val in gyro_data]

    # Update the line data
    line.set_data_3d([0, x], [0, y], [0, z])
    line.set_3d_properties(ax.get_zlim3d())

    # Redraw the canvas
    plt.draw()
    plt.pause(0.0001)

# Start the animation
ani = animation.FuncAnimation(fig, update_plot, frames=None, interval=10, blit=False)
# Keep the plot open until closed
plt.show()