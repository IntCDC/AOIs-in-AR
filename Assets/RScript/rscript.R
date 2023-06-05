
setwd("C:/Users/seyda/Documents/Uni/Master/MA/master_thesis/VideoProcessing/Assets/RScript")
library("arett")
args <- commandArgs(trailingOnly = TRUE)

for(i in 1:length(args)){

#path <- "C:/Users/seyda/Documents/Uni/Master/MA/master_thesis/VideoProcessing/Assets\\VideoProcessing/study/participant1/participant1.csv"
  path<- args[i]

filename <-paste(tools::file_path_sans_ext(basename(path)),"_fixation.csv", sep="")
newFilePath<-paste(dirname(path),"/",filename, sep="")
print(newFilePath)

data <- read.csv(file = path, header = TRUE)
data$velocity <- NA
#data$gazeHasValue <- as.logical(data$gazeHasValue)
#data[data=="True"]<- TRUE
#data[data=="False"]<- FALSE
data$gazeHasValue<-as.logical(data$gazeHasValue)
#sapply(data, class)

#filename <- paste(filename,".csv",sep="")
data_modified<-gap_fill(data, max_gap_length = 105)

data_velocity<-calculate_velocity(data_modified, window_length = 20)

data_velocity_classify<-classify_ivt(data_velocity, velocity_threshold = 100)


#data_velocity_classify2<-merge_fixations_ivt(data_velocity_classify,max_time = 101, max_angle = 0.5)


write.csv(data_velocity_classify,newFilePath, row.names = FALSE)
}
